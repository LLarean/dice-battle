# MVP backlog

Agreed on 2026-10-05: work top to bottom. Fonts are handled separately (see Deferred). Items marked *(unverified)* come from older notes and need a look at the code before work starts.

## In order

1. **Playtest the difficulty curve.** Target win rate by enemy: 100, 99, 97, 95, 88, 85, 82, 80, 68, 55%. The solver is an upper bound; soften with −1 enemy damage if it plays too hard. A full build of five special dice still solves at 91–99% against enemies 7–10.
2. **Options and credits** (owner's request, 2026-10-05). `OptionsWindow` has only the two volume sliders, the version and the Credits button; `IconButton(Info)` and `IconButton(Share)` stay hidden for the MVP (owner's decision, 2026-10-06). `CreditsWindow` is a bare window with a close button. Needed: real credits content, and working links wherever they belong (store page / share, privacy policy, contacts, credits entries) — nothing in the code calls `Application.OpenURL` yet. The owner has to supply the URLs and the credits text; all visible text goes through localization.

## Before release

- Go through the third-party assets (art, fonts, sounds, music, plugins under `Assets/`), check their licences and list them in the credits (owner's request, 2026-10-05). Known so far: the Forest Folk music pack asks for the credit "Prompt.fm"; Yuji Syuku and Ma Shan Zheng are under the SIL OFL 1.1 (licence texts lie next to the fonts).
- The music ducking under jingles and the menu track were set by numbers only — listen to them.
- `BackgroundParallax` on a phone: the tilt is now measured from how the phone is held (slowly recentred). Written without a device — check on a real phone.
- Custom keystore instead of the debug one.
- Pin `jlumbroso/free-disk-space@main` to a SHA in `.github/workflows/build.yml`.
- `com.unity.pipeline` is experimental; no tag build has run with it yet.
- WebGL has never been built.
- Old saves from before the dice redesign throw on removed enum values — decide between a version check with a wipe and leaving it.

## Optional

- `Button(Tutor)` in the main menu is a stub (the handler only logs): finish or delete.
- Short monster lines.
- A type mark on the board dice: the five face-less types have an icon on cards only.
- Mark new dice inside the inventory; the tavern badge only counts them.
- Animation polish, none of it agreed: the loot and inventory cards have no press reaction (no `BaseButton`, plain colour tint); only the main menu background has an intro zoom and sparkles, the tavern has parallax only, the battle, tournament and game over screens have a static background; `ButtonShine` is on the two Start buttons and, as the idle hint driven by `RollButtonHint`, on the two roll buttons.
- `RollButtonHint` is paused until the first roll of a turn (`DisableDiceInteractable`), so its "never rolled before → hint at once" branch in `OnEnable` never fires: a new player gets no hint on the very first roll.
- `GameOverScreen` as a window instead of a screen.

## Deferred

- **Fonts:** bake static atlases at the end (owner's decision, 2026-10-06); until then everything is dynamic. `StieglitzSP` lacks `+ •`, `ß`, `œ`, `¡¿`; the default TMP fallback covers them. Japanese uses Yuji Syuku, Chinese uses Ma Shan Zheng (see architecture); both are Regular only and were checked on the main menu only — go through the other screens for fit and readability (the Japanese title wraps mid-word).
- `AvailableLanguages` lists `SystemLanguage.Chinese`, but devices usually report `ChineseSimplified` / `ChineseTraditional`, so a Chinese device probably starts in English.
- Rejected for the MVP: HP carry-over between fights, new enemies.
