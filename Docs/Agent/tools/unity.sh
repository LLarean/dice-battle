#!/bin/bash
# Drives the Unity editor that has this project open.
# usage: unity.sh eval "<C#>" | click "<button path>" | shot <file.png> | buttons | play | stop | refresh | errors
U=~/AppData/Local/Unity/bin/unity
PROJECT="$(cd "$(dirname "$0")/../../.." && pwd -W 2>/dev/null || pwd)"
run() { "$U" command --project-path "$PROJECT" --no-banner --caller plugin --skill unity-cli "$@" 2>&1 | tail -n +2 | cut -c1-1500; }
evaluate() { for attempt in 1 2 3 4; do out=$(run eval "$1"); echo "$out" | grep -q "timed out" || break; sleep 4; done; echo "$out"; }

case "$1" in
  eval) evaluate "$2" ;;
  click) evaluate "UnityEngine.GameObject.Find(\"$2\").GetComponent<UnityEngine.UI.Button>().onClick.Invoke(); return \"ok\";" ;;
  shot) evaluate "UnityEngine.ScreenCapture.CaptureScreenshot(\"$2\"); return UnityEngine.Time.frameCount;" ;;
  buttons) evaluate "var sb = new System.Text.StringBuilder(); foreach (var b in UnityEngine.Object.FindObjectsByType<UnityEngine.UI.Button>(UnityEngine.FindObjectsSortMode.None)) { var t = b.transform; var p = t.name; while (t.parent != null) { t = t.parent; p = t.name + \"/\" + p; } sb.Append(p + \" | \" + b.interactable + \"; \"); } return sb.ToString();" ;;
  play)
    run editor_play; sleep 6
    evaluate "UnityEngine.Application.runInBackground = true; return 1;" ;;
  stop)
    evaluate "UnityEngine.Application.runInBackground = false; return 1;" > /dev/null
    run editor_stop ;;
  refresh)
    evaluate "UnityEditor.AssetDatabase.Refresh(); return 1;" > /dev/null
    for attempt in $(seq 1 20); do sleep 4; evaluate "return UnityEditor.EditorApplication.isCompiling;" | grep -q '"result":false' && break; done
    run console_status | grep -o '"compilationFailed":[a-z]*' ;;
  errors) run console -- --tail "${2:-10}" --level error ;;
esac
