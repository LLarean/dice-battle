# MVP backlog

Agreed on 2026-10-05: work top to bottom. Fonts are handled separately (see Deferred). Items marked *(unverified)* come from older notes and need a look at the code before work starts.

## In order

1. **Defeat can be dodged.** Fleeing a battle is free and restarts the fight; closing the app on the defeat screen keeps the run (the wipe happens only on the Restart button). Waits for the owner's decision: flee = defeat, wipe at the moment of defeat.
2. **Tournament shows two hints** with the same "opponent's turn" text.
3. **UI stubs** *(unverified)*: Tutorial, easter egg, Info, Share — finish or hide.
4. **Rarity and drop chances.** AdditionalDice is stronger than its tier (Golden was cut down to "any face counts 2"); LastStand, Joker and Reliable are weaker. The reward pool is a plain shuffle. Do together with 5 and 6.
5. **After loot:** two buttons, "To tavern" and "To inventory".
6. **Dice without an own face** (Reliable, Golden, Joker, AdditionalTry, AdditionalDice) look empty and differ only by colour. Needs a type badge.
7. **Sound:** victory and defeat jingles play over the music without ducking; Menu and Battle share one track.
8. **Playtest the difficulty curve.** Target win rate by enemy: 100, 99, 97, 95, 88, 85, 82, 80, 68, 55%. The solver is an upper bound; soften with −1 enemy damage if it plays too hard. A full build of five special dice still solves at 91–99% against enemies 7–10.

## Before release

- Custom keystore instead of the debug one.
- Pin `jlumbroso/free-disk-space@main` to a SHA in `.github/workflows/build.yml`.
- `com.unity.pipeline` is experimental; no tag build has run with it yet.
- WebGL has never been built.
- Old saves from before the dice redesign throw on removed enum values — decide between a version check with a wipe and leaving it.

## Optional

- Short monster lines.
- `GameOverScreen` as a window instead of a screen.

## Deferred

- **Fonts:** static atlases and CJK glyphs wait until CJK fonts are chosen. `StieglitzSP` lacks `+ •`, `ß`, `œ`, `¡¿` and all Japanese/Chinese characters; the fallback covers them for now.
- Rejected for the MVP: HP carry-over between fights, new enemies.
