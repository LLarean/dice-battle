# MVP backlog

Agreed on 2026-10-05: work top to bottom. Fonts are handled separately (see Deferred). Items marked *(unverified)* come from older notes and need a look at the code before work starts.

## In order

1. **Playtest the difficulty curve.** Target win rate by enemy: 100, 99, 97, 95, 88, 85, 82, 80, 68, 55%. The solver is an upper bound; soften with −1 enemy damage if it plays too hard. A full build of five special dice still solves at 91–99% against enemies 7–10.
2. **Options and credits** (owner's request, 2026-10-05). `OptionsWindow` has only the two volume sliders, the version and the Credits button; `IconButton(Info)` and `IconButton(Share)` stay hidden for the MVP (owner's decision, 2026-10-06). `CreditsWindow` lists every third-party asset that is in the build (see [Docs/assets.md](../assets.md)). Needed: working links wherever they belong (store page / share, privacy policy, contacts, credits entries) — nothing in the code calls `Application.OpenURL` yet. The owner has to supply the URLs; all visible text goes through localization.

## Release plan

Agreed on 2026-10-06: the first release is free, without ads, web only. itch.io goes first (no moderation, so the WebGL build can be shaken down there), Yandex Games second. Android stores come later.

- WebGL has never been built. Run the `Build` workflow with the `WebGL` platform and fix what breaks.
- Check the web build on real phones: download size and load time (the Japanese and Chinese fonts ship their source files), whether PlayerPrefs survive a browser restart, sound, the splash screen (`Loader` waits a fixed 3 s on top of the real loading), the portrait layout in a desktop browser.
- Yandex Games needs its SDK; read the platform requirements before starting *(unverified: language taken from the SDK, audio paused while the tab is hidden, no links to other stores, cloud saves through the SDK)*.
- Analytics, minimal: a thin wrapper with a provider behind it (Yandex Metrika on the web, AppMetrica on Android later). Events: first launch, battle start (enemy index), victory, defeat, loot pick (dice type), campaign completed, New Game+ start, tournament match. Needs a privacy policy link (see item 2 above).
- Store pages: itch.io asks for an AI-content disclosure per project (graphics, sound, text, code) — fill it in from the asset list, which gets an "AI: yes / no / unknown" column.
- Promo materials: icon, cover, screenshots, a gameplay GIF. Screenshots and the GIF can be captured from the editor; the cover needs art.

## Before release

- Go through the third-party assets (art, fonts, sounds, music, plugins under `Assets/`), check their licences and list them in the credits (owner's request, 2026-10-05). The list with sources, licences and credit obligations is in [Docs/assets.md](../assets.md); what is left is its "Open questions" section.
- The music ducking under jingles and the menu track were set by numbers only — listen to them.
- `BackgroundParallax` on a phone: the tilt is now measured from how the phone is held (slowly recentred). Written without a device — check on a real phone.
- Custom keystore instead of the debug one.
- `com.unity.pipeline` is experimental; no tag build has run with it yet.
- Old saves from before the dice redesign throw on removed enum values — decide between a version check with a wipe and leaving it.

## Optional

- Short monster lines.
- A type mark on the board dice: the five face-less types have an icon on cards only.
- Mark new dice inside the inventory; the tavern badge only counts them.
- Animation polish, none of it agreed: the loot and inventory cards have no press reaction (no `BaseButton`, plain colour tint); only the main menu background has an intro zoom, the tavern has parallax and sparkles, the battle, tournament and game over screens have a static background; `ButtonShine` is on the two Start buttons and, as the idle hint driven by `RollButtonHint`, on the two roll buttons; the board dice pulse until the first die is ever tapped.
- `GameOverScreen` as a window instead of a screen.

## Deferred

- **Fonts:** bake static atlases at the end (owner's decision, 2026-10-06); until then everything is dynamic. `StieglitzSP` lacks `+ •`, `ß`, `œ`, `¡¿`; the default TMP fallback covers them. Its accented Latin letters are drawn without the accents (`ZURUCK`, `ESPANOL`, `HEROIS`), which misspells German, French, Portuguese and Spanish — needs a font with real diacritics or a decision to live with it. Japanese uses Yuji Syuku, Chinese uses Ma Shan Zheng (see architecture); both are Regular only and were checked on the main menu only — go through the other screens for fit and readability.
- **Sound pass** (owner's decision, 2026-10-07: skipped for the MVP; 2026-10-08: attack sounds added as a trial, see architecture, "Audio"). The rest stays deferred.
  - Listen to the trial: clip choice per class and enemy, the three delays in `GameLogic` (`_playerHitDelay`, `_enemyAttackDelay`, `_enemyHitDelay`), volume against the music. The skeletons share the knight's and the archer's clips. Neither side has a swing animation yet, only a reaction to being hit.
  - `PlayerHeal`, `PlayerArmor` and `EnemyDefeated` are raised by `GameLogic` but have no clips.
  - Agreed first candidates, each a new `SoundType` (append at the end of the enum — `SoundConfig` stores the values by index): critical hit (the `IsCritical` branch of `ApplyPlayerAttack`; check that it does not collide with the `Victory` jingle), loot window opening, equipping and unequipping a die (now `Click`).
  - `TournamentLogic` raises no sounds at all. `PlayerAttack` and `GameOver` have clips but are never raised; `EnemyHit` and `EnemySpawn` have neither.
  - Further ideas, none agreed: Last Stand and Golden triggers, a rarity accent on loot, class change, tavern door, an ambience loop under the music (needs a second channel in `AudioPlayer`), a separate dragon or tournament track.
  - Every clip that goes in gets a row in [Docs/assets.md](../assets.md).
- Rejected for the MVP: HP carry-over between fights, new enemies.

## If the game finds an audience

The owner thinking aloud on 2026-10-09 — not a plan, nothing agreed, and only worth a look if players actually show up.

- A real ladder against other players with a shared rating, in the spirit of Hearthstone. No paid advantages.
- Deck building reworked: dice are earned through small separate campaigns, each of which can carry a story.
- What it would mean for the code (agent's note): accounts, rating and matchmaking need a backend, rolls have to be validated on the server, saves have to leave PlayerPrefs. The closest thing today is the tournament (`TournamentLogic`, the standard deck from `DiceRuleset.SetStandard`), which plays against a bot.
