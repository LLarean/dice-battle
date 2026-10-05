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
| `Localization` | `LocKeys` (all keys as constants), `LocalizedTMP` |
| `Editor` | `DebugOptionsEditor` (groups debug buttons by the `Group:` prefix of the button text) |

## Communication

Components do not reference each other across features. They talk through `GameSignals.SignalSystem`:

```csharp
SignalSystem.Raise<ISoundHandler>(handler => handler.PlaySound(SoundType.Click));
```

A handler subscribes in `Awake`/`Start` and unsubscribes in `OnDestroy`. Screens raise signals from `OnEnable`, which can run before a handler's `Start` — a handler that subscribes in `Start` misses them (see the comment in `Hint.Start`).

## Screens

`ScreenChanger` owns everything under `RootUI`. Screens replace each other (`ShowScreen`), windows stack on top (`ShowWindow` / `CloseTopWindow`). Input is locked for `TransitionLockDuration` after a change. Each screen starts its logic and music in `OnEnable`.

Flow: `SplashScreen` → `MainMenuScreen` → `TavernScreen` → `GameScreen` (campaign battle) → `LootScreen` (window, pick 1 of 2) → tavern. Tavern also leads to `InventoryScreen` and `TournamentPyramidScreen` → `TournamentScreen`.

## Dice rules

- A die has a `DiceType` (its effect) and rolls a `DiceValue` face: Empty, Attack, Defense, Heal.
- Each equipped inventory item is one physical die; free slots are `Default` dice. `DiceRuleset.Deck` builds the board order. Tournament swaps in a standard deck via `DiceRuleset.SetStandard`.
- `DiceResult` is the single source of truth for what a roll is worth. Effects trigger only on the die's own face and are additive:
  - Sharp / Sturdy / Healing: own face counts 2
  - Vampiric: Attack also heals 1; Thorns: Defense also deals 1
  - Reliable: never rolls Empty; Joker: Empty becomes the most common other face
  - Golden: +1 to every die showing the same face as it
  - LastStand: survive a lethal hit once per battle at 1 HP
  - AdditionalTry: +1 reroll; AdditionalDice: +1 board slot (not a physical die)
- 5 or more Attack faces is a critical: instant kill. The owner wants this kept.
- Rarity (`GetRarity`): Legendary = LastStand; Rare = Golden, Joker, AdditionalDice; Uncommon = Reliable, Thorns, Vampiric, AdditionalTry; the rest Common. Only Legendary has a glow.

## Campaign

10 enemies in `GameConfig.asset`, three player classes (Knight, Archer, Mage). After the dragon, New Game+ scales enemy HP only (`Spawner`). Loot after each victory: `GameData.GetRandomRewards(level * 2, 2)` from a shuffled pool saved in `AvailableRewardsPool`; the pending reward index survives an app kill (`SetPendingLootReward`).

An unfinished battle is saved (`BattleSaveData`) when `GameConfig.CanSaveBattle` is set and restored by `GameScreen.OnEnable`.

## Saves

Everything is PlayerPrefs (`PlayerPrefsKeys`). There is no migration: a breaking change needs `DebugOptions` → reset. `GameData.ResetAll` is the full wipe.

## Audio

`AudioPlayer` has two music sources and crossfades between them (`_musicFadeDuration`); each track resumes where it stopped. Tracks are identified by clip, so two `SoundType`s mapped to the same clip do not restart it. Menu and Battle currently share one clip. SFX go through `PlayOneShot` with a random pitch.

## Debug

`DebugOptions` (scene object, editor only) has buttons for battle, campaign, loot, inventory, tournament, innkeeper and saves. `DebugOverrides` can force dice faces.
