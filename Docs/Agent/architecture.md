# Architecture

A map, not a spec. Paths are relative to `Assets/_DiceBattle/Scripts`.

## Layout

| Folder | What lives there |
| --- | --- |
| `Core` | Battle rules and flow: `GameLogic` (campaign), `TournamentLogic` (tournament), `Dice`, `DiceHolder`, `DiceResult`, `DiceRuleset`, `Spawner`, `TournamentBracket` |
| `UI/Screens`, `UI/Windows` | One class per screen/window; `UI/Infrastructure` has `ScreenChanger` and `ScreenType` |
| `UI/Components`, `UI/Units` | Reusable widgets (`InventoryItem`, `Hint`, `Innkeeper`, `UnitPanel`, ...) |
| `Global` | Persistent state behind static facades: `GameData`, `GameSettings`, `BattleSaveData`, `Rewards/AvailableRewardsPool` |
| `Inventory` | `Inventory`, `Item`, `ItemsStorage` |
| `Data` | ScriptableObject types: `GameConfig`, `UnitConfig`, `SoundConfig`; assets in `Assets/_DiceBattle/Data` |
| `Events` | Signal interfaces (`ISoundHandler`, `IScreenHandler`, `IHintHandler`, `ITopBarHandler`, ...) |
| `Animations` | LeanTween helpers; `DiceAnimation` is the shared roll animation |
| `Audio` | `AudioPlayer`, `SoundType` |
| `Auxiliary` | Extensions, palettes (`DiceTypeColors`, `DiceRarityColors`), `DebugOptions` (editor only) |
| `Localization` | `LocKeys` (all keys as constants), `LocalizedTMP`, `CjkFontFallback` (scene object of the same name: on a language change makes `YujiSyuku-Regular SDF` for Japanese or `MaShanZheng-Regular SDF` for Chinese the only CJK fallback of the fonts in `_mainFonts` (`StieglitzSP-Bold 2 SDF` and `LiberationSans SDF`, which the dice hints and credits use), because the two languages share code points but draw them differently) |
| `Editor` | `DebugOptionsEditor` (groups debug buttons by the `Group:` prefix of the button text) |

## Communication

Components do not reference each other across features. They talk through `GameSignals.SignalSystem`:

```csharp
SignalSystem.Raise<ISoundHandler>(handler => handler.PlaySound(SoundType.Click));
```

A handler subscribes in `Awake`/`Start` and unsubscribes in `OnDestroy`. Screens raise signals from `OnEnable`, which can run before a handler's `Start` — a handler that subscribes in `Start` misses them (`Hint` subscribes in `Awake` for this reason). The same goes for a screen that is activated for the first time: its `OnEnable` runs before the `Awake` of its children, so anything it calls on them there must not rely on their `Awake` (`InventoryItem.ContentGroup` is lazy for this reason).

## Screens

`ScreenChanger` owns everything under `RootUI`. Screens replace each other (`ShowScreen`), windows stack on top (`ShowWindow` / `CloseTopWindow`). Input is locked for `TransitionLockDuration` after a change. Each screen starts its logic and music in `OnEnable`.

Flow: `SplashScreen` → `MainMenuScreen` → `TavernScreen` → `GameScreen` (campaign battle) → `LootScreen` (window, pick 1 of 3) → tavern. Tavern also leads to `InventoryScreen` and `TournamentPyramidScreen` → `TournamentScreen`.

Main menu easter egg: tapping a die rerolls it; when all dice show the same face an extra die drops in and all are rethrown (`MainMenuScreen.TriggerEasterEgg`). At `_maxDiceCount` a match plays the Reward jingle and goes back to three; reopening the menu also resets.

## Dice rules

- A die has a `DiceType` (its effect) and rolls a `DiceValue` face: Empty, Attack, Defense, Heal.
- Each equipped inventory item is one physical die; free slots are `Default` dice. `DiceRuleset.Deck` builds the board order. Tournament swaps in a standard deck via `DiceRuleset.SetStandard`.
- `DiceResult` is the single source of truth for what a roll is worth. Effects trigger only on the die's own face and are additive:
  - Sharp / Sturdy / Healing: own face counts 2
  - Vampiric: Attack also heals 1; Thorns: Defense also deals 1
  - Reliable: never rolls Empty; Joker: always counts as the most common non-empty face of the other dice (tie → leftmost; nothing to copy → its own roll). `DiceHolder.RefreshFaces` shows that face once the die is in its slot, and it changes live when the others are rerolled
  - Golden: any own face counts 2 as it
  - LastStand: survive a lethal hit once per battle at 1 HP
  - AdditionalTry: +1 reroll; AdditionalDice: +1 board slot (not a physical die)
- 5 or more Attack faces is a critical: instant kill. The owner wants this kept.
- Rarity (`GetRarity`): Legendary = AdditionalDice; Rare = Golden, AdditionalTry, LastStand; Uncommon = Thorns, Vampiric, Joker; the rest Common. Only Legendary has a glow. Rarity sets the drop weight (`GetDropWeight`: 10 / 6 / 3 / 2).
- On cards (loot, inventory) a die shows a fixed face from `GetIconCategory`. Types without an own face (Reliable, Golden, Joker, AdditionalTry, AdditionalDice) use the type sprites `Art/Sources/Dices/dice_reliable|golden|joker|reroll|extra.png` (`Dice._typeSprites`), cut from Kenney board-game icons. On the board these dice still differ only by colour.

## Campaign

10 enemies in `GameConfig.asset`, three player classes (Knight, Archer, Mage). After the dragon, New Game+ scales enemy HP only (`Spawner`). Loot after each victory: `GameData.GetRandomRewards(level * 3, 3)` (the count is the number of cards on `LootScreen`). `AvailableRewardsPool` is a flat saved list of offers that grows on demand; each offer is three different types drawn by rarity weight, so reopening the loot of a level shows the same cards. The pending reward index survives an app kill (`SetPendingLootReward`). Picking a die raises `GameData.NewDiceCount`, shown as a red badge on the tavern inventory button until the inventory is opened.

An unfinished battle is saved (`BattleSaveData`) when `GameConfig.CanSaveBattle` is set and restored by `GameScreen.OnEnable`. It is written at the start of a battle, when a roll starts (attempt already spent, old faces), when it lands (new faces) and at the end of a turn, so quitting never gives a roll back.

Dodging a campaign defeat is left in on purpose (owner's decision, 2026-10-05): fleeing is free, and closing the app on the defeat screen keeps the run. The tournament is strict: fleeing or closing the app mid-match is a defeat (`TournamentBracket.IsMatchInProgress`), and a defeat ends the bracket.

## Saves

Everything is PlayerPrefs (`PlayerPrefsKeys`). There is no migration: a breaking change needs `DebugOptions` → reset. `GameData.ResetAll` is the full wipe.

## Audio

`AudioPlayer` has two music sources and crossfades between them (`_musicFadeDuration`); each track resumes where it stopped. Tracks are identified by clip, so two `SoundType`s mapped to the same clip do not restart it. Menu, Tavern and Battle each have their own track in `SoundConfig.asset`. SFX go through `PlayOneShot` with a random pitch. The Victory, Defeat and Reward jingles duck the music to `_duckedMusicVolume` for the length of the clip.

## Debug

`DebugOptions` (scene object, editor only) has buttons for battle, campaign, loot, inventory, tournament, innkeeper and saves. `DebugOverrides` can force dice faces.
