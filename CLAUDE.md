# DiceBattle — agent notes

Small mobile dice battler (Unity 6000.0.61f1, uGUI, portrait 1080x1920). One scene: `Assets/_DiceBattle/Scenes/Main.unity`. The goal is a small but solid game — avoid scope growth.

Keep these docs current: when a change makes a statement here wrong, fix the statement in the same commit. Move finished backlog items out instead of ticking them.

## Where to look

- [Docs/Agent/architecture.md](Docs/Agent/architecture.md) — code map, screen flow, dice rules, saves, audio
- [Docs/Agent/unity-workflow.md](Docs/Agent/unity-workflow.md) — driving the editor from the CLI, verifying in Play mode, pitfalls
- [Docs/Agent/backlog.md](Docs/Agent/backlog.md) — what is left for the MVP, in agreed order, plus deferred items
- `Docs/Plan-Part1-MVP.md`, `Docs/Plan-Part2-Expansion.md` — the owner's original plans (do not edit)

## Rules of the road

- All game code lives in `Assets/_DiceBattle/Scripts`. Third-party folders under `Assets/` are not ours.
- Private fields and constants are `_camelCase`; comparisons with booleans are written `== false`; braces on every block.
- No comments unless the logic is non-obvious; comments in English.
- Player-facing text goes through localization (`LocKeys` + CSV in `Assets/SimpleLocalization/Resources/Localization/`, 8 languages). Never hardcode a visible string.
- Scene and prefab changes are made through the editor (CLI `eval`), never by hand-editing YAML.
- The two TMP font assets (`StieglitzSP-Bold 2 SDF`, `LiberationSans SDF - Fallback`) get dirty after every Play session (dynamic atlas). Do not commit them unless asked.
- Verify UI changes in Play mode and look at a screenshot before reporting them done. Sound cannot be verified by the agent — say so.
- Play-mode checks use the developer's real save (PlayerPrefs). Avoid picking loot or finishing battles unless the task needs it.
