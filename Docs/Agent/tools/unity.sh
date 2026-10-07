#!/bin/bash
# Drives the Unity editor that has this project open.
# usage: unity.sh eval "<C#>" | evalfile <file.cs> | click "<button path>" | shot <file.png> | buttons | play | stop | refresh | errors
U=~/AppData/Local/Unity/bin/unity
PROJECT="$(cd "$(dirname "$0")/../../.." && pwd -W 2>/dev/null || pwd)"
# Exists while a Play session started by "play" is running, so "stop" never ends the owner's session.
MARKER="${TMPDIR:-/tmp}/dicebattle-agent-play"
run() { "$U" command --project-path "$PROJECT" --no-banner --caller plugin --skill unity-cli "$@" 2>&1 | tail -n +2 | cut -c1-1500; }
# Without --timeout an eval gives up on a busy main thread after 5 s and logs an error in the editor console.
evaluate() { for attempt in 1 2 3 4; do out=$(run eval "$1" -- --timeout 25000); echo "$out" | grep -q "timed out" || break; sleep 4; done; echo "$out"; }

case "$1" in
  eval) evaluate "$2" ;;
  evalfile) evaluate "$(cat "$2")" ;;
  click) evaluate "UnityEngine.GameObject.Find(\"$2\").GetComponent<UnityEngine.UI.Button>().onClick.Invoke(); return \"ok\";" ;;
  shot) evaluate "UnityEngine.ScreenCapture.CaptureScreenshot(\"$2\"); return UnityEngine.Time.frameCount;" ;;
  buttons) evaluate "var sb = new System.Text.StringBuilder(); foreach (var b in UnityEngine.Object.FindObjectsByType<UnityEngine.UI.Button>(UnityEngine.FindObjectsSortMode.None)) { var t = b.transform; var p = t.name; while (t.parent != null) { t = t.parent; p = t.name + \"/\" + p; } sb.Append(p + \" | \" + b.interactable + \"; \"); } return sb.ToString();" ;;
  play)
    if evaluate "return UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode;" | grep -q '"result":true'; then
      [ -f "$MARKER" ] && { echo "already playing (started by the agent)"; exit 0; }
      echo "REFUSED: the owner has the editor in Play mode"; exit 1
    fi
    touch "$MARKER"
    run editor_play; sleep 6
    evaluate "UnityEngine.Application.runInBackground = true; return 1;" ;;
  stop)
    [ -f "$MARKER" ] || { echo "REFUSED: Play mode was not started by the agent"; exit 1; }
    rm -f "$MARKER"
    evaluate "UnityEngine.Application.runInBackground = false; return 1;" > /dev/null
    run editor_stop ;;
  refresh)
    evaluate "UnityEditor.AssetDatabase.Refresh(); return 1;" > /dev/null
    for attempt in $(seq 1 20); do sleep 4; evaluate "return UnityEditor.EditorApplication.isCompiling;" | grep -q '"result":false' && break; done
    run console_status | grep -o '"compilationFailed":[a-z]*' ;;
  errors) run console -- --tail "${2:-10}" --level error ;;
esac
