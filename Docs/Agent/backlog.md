# MVP backlog

Agreed on 2026-10-05: work top to bottom. Fonts are handled separately (see Deferred). Items marked *(unverified)* come from older notes and need a look at the code before work starts.

## In order

1. **Playtest the difficulty curve by hand.** Target win rate by enemy: 100, 99, 97, 95, 88, 85, 82, 80, 68, 55%. The numbers come from `Docs/Agent/tools/balance.py` (exact per battle, averaged over simulated campaigns with loot); the owner still has to play it. On 2026-10-12 the damage of enemies 4–8 went down by 1 (now 2, 4, 8, 8, 7, 9, 9, 8, 8, 8): before that a perfectly played knight won 76–80% against enemies 4–6 and a casual one 28–51%. What the solver says now, perfect / casual player, mean of the three classes: enemy 4 — 99 / 95, 5 — 98 / 89, 6 — 97 / 75, 7 — 99 / 67, 8 — 100 / 73, 9 — 95 / 26, 10 — 92 / 22. The extra die taking a slot of its own (2026-10-14) moved enemies 9–10 by 1–2 points and nothing else. Known and left open:
   - One point of damage moves the win rate by 20–30 points, so damage alone cannot hit the curve. The casual player falls off a cliff at enemy 9 (armour 2) while the perfect one barely notices enemies 9–10; health or armour of the last two enemies is the finer lever.
   - The knight is the weakest class in the middle, the mage at the end (casual: 12% against the dragon).
   - A perfect player never keeps a Joker and almost never Healing, Vampiric or Reliable; the builds are Golden, Sharp, AdditionalTry, LastStand.
2. **Options and credits** (owner's request, 2026-10-05). `OptionsWindow` has only the two volume sliders, the version and the Credits button; `IconButton(Info)` and `IconButton(Share)` stay hidden for the MVP (owner's decision, 2026-10-06). `CreditsWindow` lists every third-party asset that is in the build (see [Docs/assets.md](../assets.md)). Needed: working links wherever they belong (store page / share, privacy policy, contacts, credits entries) — nothing in the code calls `Application.OpenURL` yet. The owner has to supply the URLs; all visible text goes through localization.

## Release plan

Agreed on 2026-10-06: the first release is free, without ads, web only. itch.io goes first (no moderation, so the WebGL build can be shaken down there), Yandex Games second. Android stores come later.

- The WebGL build runs on CI: a manual `Build` run with the `WebGL` platform (first green one: run 11, 2026-10-14) uploads the `DiceBattle-<version>-b<run>-WebGL` artifact and publishes the same build to GitHub Pages (http://llarean.org/dice-battle/, public). The build is set to Gzip with decompression fallback: itch.io does not send the headers Brotli needs, and the fallback makes the build start on any host without server setup. The page is our own template `Assets/WebGLTemplates/Portrait`: the canvas keeps 9:16 on a wide window and fills a phone screen, fullscreen puts black bars around it instead of stretching the game to landscape.
- itch.io page (started 2026-10-15). The build goes up by hand: the artifact zip holds a folder `DiceBattle-<version>-b<run>/`, while itch wants `index.html` at the root of the zip, so zip that folder's contents. Kind of project HTML, "played in the browser", viewport 540x960, mobile friendly, portrait, fullscreen button. Uploading from CI with butler was considered and put off until itch updates get frequent (owner's decision, 2026-10-15). Check whether the saves survive uploading a new build: Unity keeps them in IndexedDB, and itch serves every upload under a new path.
- Check the web build on real phones: download size and load time (the Japanese and Chinese fonts are cut to the used characters, 13.6 MB down to 1.2 MB), whether PlayerPrefs survive a browser restart, sound, the splash screen (`Loader` waits a fixed 3 s on top of the real loading), the portrait layout in a desktop browser.
- Yandex Games needs its SDK; read the platform requirements before starting *(unverified: language taken from the SDK, audio paused while the tab is hidden, no links to other stores, cloud saves through the SDK)*.
- Analytics, minimal: a thin wrapper with a provider behind it (Yandex Metrika on the web, AppMetrica on Android later). Events: first launch, battle start (enemy index), victory, defeat, loot pick (dice type), campaign completed, New Game+ start, tournament match. Needs a privacy policy link (see item 2 above).
- Store pages: itch.io asks for an AI-content disclosure per project (graphics, sound, text, code) — fill it in from the asset list, which gets an "AI: yes / no / unknown" column.
- Promo materials: icon, cover, screenshots, a gameplay GIF, store texts. The folders and what each one still lacks are in [Docs/Promo](../Promo/README.md). Screenshots and the GIF can be captured from the editor; the cover needs art.
- Windows build (owner's request, 2026-10-13): a free download next to the APK, and a quicker way to playtest the balance. The `Build` workflow takes `StandaloneWindows64` and a `v*` tag attaches a zip to the release; neither has run on CI yet. The desktop icon is the default one (`dice_sword.png`).

## Before release

- Go through the third-party assets (art, fonts, sounds, music, plugins under `Assets/`), check their licences and list them in the credits (owner's request, 2026-10-05). The list with sources, licences and credit obligations is in [Docs/assets.md](../assets.md); what is left is its "Open questions" section.
- Custom keystore instead of the debug one.
- `com.unity.pipeline` is experimental; no tag build has run with it yet.

## Optional

- Short monster lines.
- Mark new dice inside the inventory; the tavern badge only counts them.
- Animation polish, none of it agreed: the loot and inventory cards have no press reaction (no `BaseButton`, plain colour tint); only the main menu background has an intro zoom, the tavern has parallax and sparkles, the battle, tournament and game over screens have a static background; `ButtonShine` is on the two Start buttons and, as the idle hint driven by `RollButtonHint`, on the two roll buttons; the board dice pulse until the first die is ever tapped.
- `GameOverScreen` as a window instead of a screen.

## Deferred

- **Fonts:** bake static atlases at the end (owner's decision, 2026-10-06); until then everything is dynamic. Glyph coverage was checked against the source font files for every character of the localization CSVs (2026-10-14): nothing is missing in any of the 8 languages, Yuji Syuku and Ma Shan Zheng cover every kana, kanji, hanzi and full-width mark the texts use. What is left:
  - `StieglitzSP` maps every accented Latin letter to its plain base glyph and has no combining marks: all 27 accented letters in the texts lose the accent (`ZURUCK`, `ESPANOL`, `HEROIS`), in titles and buttons too, so German, French, Portuguese and Spanish are misspelt. Needs a font with real diacritics or a decision to live with it. Russian is fine (`й`, `ё` have their own glyphs).
  - Glyphs `StieglitzSP` lacks are drawn by LiberationSans (or the CJK font) in mid-word: `•` (help text, all languages), `ß` (German, 14 keys), `œ` (French, 2), `¡` and `¿` (Spanish, 13 and 20, mostly the innkeeper), `…` (Japanese and Chinese splash tip), `/` (Chinese `ui.message.dice_slots`). The other way round, `StieglitzSP` draws Latin letters, digits, `!` and the Chinese curly quotes `“ ”` inside CJK text.
  - `StieglitzSP` has no fallback of its own; it reaches LiberationSans only through the default font in `TMP Settings`. Changing that default breaks the fallback.
  - Both CJK fonts are Regular only and were looked at on the main menu only; the other screens still need a pass for fit and readability.
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
