# MVP backlog

Agreed on 2026-10-05: work top to bottom. Fonts are handled separately (see Deferred). Items marked *(unverified)* come from older notes and need a look at the code before work starts.

## In order

4. **Sound:** victory and defeat jingles play over the music without ducking; Menu and Battle share one track.
5. **Playtest the difficulty curve.** Target win rate by enemy: 100, 99, 97, 95, 88, 85, 82, 80, 68, 55%. The solver is an upper bound; soften with −1 enemy damage if it plays too hard. A full build of five special dice still solves at 91–99% against enemies 7–10.
6. **Options and credits** (owner's request, 2026-10-05). `OptionsWindow` has only the two volume sliders, the version and the Credits button; `IconButton(Info)` and `IconButton(Share)` are inactive and their handlers just close the window. `CreditsWindow` is a bare window with a close button. Needed: real credits content, and working links wherever they belong (store page / share, privacy policy, contacts, credits entries) — nothing in the code calls `Application.OpenURL` yet. The owner has to supply the URLs and the credits text; all visible text goes through localization.
7. **Menu dice easter egg** (owner's request, 2026-10-05). Clicking the dice on the main menu rerolls them; when all show the same face `MainMenuScreen.TriggerEasterEgg` only logs. Give it a real reaction. What it does is not decided yet.

## Before release

- Go through the third-party assets (art, fonts, sounds, music, plugins under `Assets/`), check their licences and list them in the credits (owner's request, 2026-10-05).
- `BackgroundParallax` on a phone: the tilt is now measured from how the phone is held (slowly recentred). Written without a device — check on a real phone.
- Custom keystore instead of the debug one.
- Pin `jlumbroso/free-disk-space@main` to a SHA in `.github/workflows/build.yml`.
- `com.unity.pipeline` is experimental; no tag build has run with it yet.
- WebGL has never been built.
- Old saves from before the dice redesign throw on removed enum values — decide between a version check with a wipe and leaving it.

## Optional

- `Button(Tutor)` in the main menu is a stub (the handler only logs): finish or delete.
- Short monster lines.
- Animation polish, none of it agreed: the loot and inventory cards have no press reaction (no `BaseButton`, plain colour tint); only the main menu background has an intro zoom and sparkles, the tavern has parallax only, the battle, tournament and game over screens have a static background; `ButtonShine` is only on the two Start buttons.
- `GameOverScreen` as a window instead of a screen.

## Deferred

- **Fonts:** static atlases and CJK glyphs wait until CJK fonts are chosen. `StieglitzSP` lacks `+ •`, `ß`, `œ`, `¡¿` and all Japanese/Chinese characters; the fallback covers them for now.
- Rejected for the MVP: HP carry-over between fights, new enemies.
