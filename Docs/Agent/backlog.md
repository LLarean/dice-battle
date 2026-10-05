# MVP backlog

Agreed on 2026-10-05: work top to bottom. Fonts are handled separately (see Deferred). Items marked *(unverified)* come from older notes and need a look at the code before work starts.

## In order

1. **Golden breaks the curve.** A build with one Golden wins about 99% even against the dragon. Needs a nerf.
2. **Two exploits** *(unverified)*: fleeing a battle is free; quitting mid-turn gives an extra reroll.
3. **"Осталось 1 попыток"** — the attempts label has no plural forms.
4. **Tournament shows two hints** with the same "opponent's turn" text.
5. **UI stubs** *(unverified)*: Tutorial, easter egg, Info, Share — finish or hide.
6. **Rarity and drop chances.** Golden and AdditionalDice are stronger than their tier; LastStand, Joker and Reliable are weaker. The reward pool is a plain shuffle. Do together with 7 and 8.
7. **After loot:** two buttons, "To tavern" and "To inventory".
8. **Dice without an own face** (Reliable, Golden, Joker, AdditionalTry, AdditionalDice) look empty and differ only by colour. Needs a type badge.
9. **Sound:** victory and defeat jingles play over the music without ducking; Menu and Battle share one track.
10. **Playtest the difficulty curve.** Target win rate by enemy: 100, 99, 97, 95, 88, 85, 82, 80, 68, 55%. The solver is an upper bound; soften with −1 enemy damage if it plays too hard.

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
