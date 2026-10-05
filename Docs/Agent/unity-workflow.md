# Working with the editor

The agent cannot see or hear the game directly. It drives the open editor through the Unity CLI and looks at screenshots.

## Helper

`Docs/Agent/tools/unity.sh` wraps the CLI (run it from Git Bash):

```bash
Docs/Agent/tools/unity.sh refresh                 # import + compile, prints compilationFailed
Docs/Agent/tools/unity.sh play                    # enter Play mode, keep it running in background
Docs/Agent/tools/unity.sh click "RootUI/MainMenuScreen/Bottom/Button(Start)"
Docs/Agent/tools/unity.sh shot "C:/path/to/shot.png"
Docs/Agent/tools/unity.sh eval "return UnityEngine.Time.frameCount;"
Docs/Agent/tools/unity.sh buttons                 # every Button with its path and interactable flag
Docs/Agent/tools/unity.sh errors 10
Docs/Agent/tools/unity.sh stop
```

Screenshots and logs go to the session scratchpad, never into the repo.

## Pitfalls

- Another Unity project may be open on the same machine; the helper always passes `--project-path`.
- If the owner has the editor in Play mode, do not stop it. Unity defers compilation while playing — report and wait.
- `eval` right after a refresh or after entering Play mode can fail with "Main thread operation timed out"; the helper retries. A timed-out eval may still have run — check state before repeating anything with side effects.
- In `eval` code use escaped double-quoted strings. Char literals and `bool + char` do not compile there.
- Play mode pauses when the editor loses focus unless `Application.runInBackground` is true. `play` sets it, `stop` restores it.
- A round trip takes seconds, so timing cannot be observed by polling. To watch an animation or a fade, schedule the actions and a per-frame logger inside one `eval` (`LeanTween.delayedCall` + `LeanTween.value(...).setOnUpdate`), write to a file, read it afterwards.
- `ScreenCapture.CaptureScreenshot` is written at the end of the frame; wait a second before reading the file.
- Python on Windows does not see Git Bash's `/tmp`; use the scratchpad path.
- `DebugOptions._needResetAll` is on in the scene: every Play start wipes all saves. To test anything that survives a restart, set the field to false through reflection in edit mode (do not save the scene) and set it back afterwards.

## Useful paths in the scene

- Buttons: `RootUI/MainMenuScreen/Bottom/Button(Start)`, `RootUI/TavernScreen/Button(Start)`, `RootUI/TavernScreen/Button(Tournament)`, `RootUI/TavernScreen/IconButton(Inventory)`, `RootUI/GameScreen/Bot/Button(Context)`, `RootUI/GameScreen/Bot/Button(All)`, `RootUI/TournamentPyramidScreen/Button(Context)`, `RootUI/TournamentScreen/Bot/Button(Context)`, `RootUI/TopBar/Panel/Button(Back)`, `RootUI/ConfirmWindow/Substrate/Window/LabelButton(Accept)`
- Board dice: `RootUI/GameScreen/GameBoard/DiceHolder`; loot cards: `RootUI/LootScreen/InventoryItem_0`, `_1`, `_2`
- Music sources: both `AudioSource`s on the `Music` object

## Recipes

- **Show loot without winning:** `GameData.SetPendingLootReward(level)`, toggle `RootUI/LootScreen` off and on, then `GameData.ClearPendingLootReward()`. Do not pick a card — it adds a die to the save.
- **See any die type on a card:** in the inventory, call `InventoryItem.Initialize(new DiceBattle.UI.Item { Type = ... })` on the existing cards. Nothing is saved.
- **Simulate a tap or a hold:** `ExecuteEvents.Execute(go, new PointerEventData(EventSystem.current), ExecuteEvents.pointerDownHandler)` and `pointerUpHandler`; `click` only invokes `onClick` and skips the press handlers.
- **Edit a prefab:** `PrefabUtility.LoadPrefabContents` → change → `SaveAsPrefabAsset` → `UnloadPrefabContents`.
- **Edit the scene:** change the object in edit mode, `EditorSceneManager.MarkSceneDirty`, `SaveOpenScenes`. Check `git diff` for unrelated hunks.
- **Check text fits in every language:** iterate `LocalizationManager.Dictionary` and compare `TMP_Text.GetPreferredValues(text, width, 0)` with the rect.

## Git

- Small commits per stage, pushed to `origin main`. CI builds only on `v*` tags or manually.
- `core.autocrlf` is on; the LF/CRLF warnings are noise.
- To split scene hunks between commits: save `git diff` to a patch, filter hunks, `git apply --cached`.
