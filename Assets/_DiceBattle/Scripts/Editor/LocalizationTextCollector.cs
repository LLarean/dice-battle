using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DiceBattle.Localization.Editor
{
    /// <summary>
    /// Collects hardcoded text (scenes, prefabs, ScriptableObject configs) and already used
    /// localization keys (LocalizedTMP component, LocalizationManager.Localize calls in code) into one CSV.
    /// </summary>
    public static class LocalizationTextCollector
    {
        private const string ScriptsRoot = "Assets/_DiceBattle/Scripts";

        // LocalizationManager.Localize(<arg>) — does not match inside // or /* */ comments.
        private static readonly Regex LocalizeCallRegex = new Regex(
            @"(?<!//.*)LocalizationManager\s*\.\s*Localize\s*\(\s*(?<arg>[^)]+?)\s*\)",
            RegexOptions.Compiled);

        // public const string Name = "value";
        private static readonly Regex ConstDeclRegex = new Regex(
            @"const\s+string\s+(?<name>\w+)\s*=\s*""(?<value>(?:[^""\\]|\\.)*)""",
            RegexOptions.Compiled);

        // class/struct Name — used to track nesting for qualified constant lookup (LocKeys.Button.EndTurn).
        private static readonly Regex TypeDeclRegex = new Regex(
            @"\b(?:class|struct)\s+(?<name>\w+)",
            RegexOptions.Compiled);

        private class Row
        {
            public string Key;
            public string Source;
            public string Text;
        }

        [MenuItem("Tools/Localization/Collect Hardcoded Text To CSV")]
        public static void Collect()
        {
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                Debug.LogWarning("Localization export cancelled: unsaved scene changes.");
                return;
            }

            var path = EditorUtility.SaveFilePanel("Save localization CSV", Application.dataPath, "LocalizationExport", "csv");

            if (string.IsNullOrEmpty(path)) return;

            Run(path);
            EditorUtility.RevealInFinder(path);
        }

        /// <summary>
        /// CLI entry point: -executeMethod DiceBattle.Localization.Editor.LocalizationTextCollector.CollectBatch -exportPath &lt;file.csv&gt;
        /// </summary>
        public static void CollectBatch()
        {
            var args = System.Environment.GetCommandLineArgs();
            var index = System.Array.IndexOf(args, "-exportPath");
            var path = index >= 0 && index + 1 < args.Length
                ? args[index + 1]
                : Path.Combine(Application.dataPath, "..", "LocalizationExport.csv");

            Run(path);
        }

        private static void Run(string path)
        {
            var rows = new List<Row>();

            CollectFromScenes(rows);
            CollectFromPrefabs(rows);
            CollectFromConfigs(rows);
            CollectFromCode(rows);

            var deduped = Dedupe(rows);
            WriteCsv(path, deduped);

            Debug.Log($"Localization export done: {deduped.Count} rows ({rows.Count} before dedupe) -> {path}");
        }

        private static void CollectFromScenes(List<Row> rows)
        {
            var sceneGuids = AssetDatabase.FindAssets("t:Scene", new[] { "Assets/_DiceBattle" });
            var openScenePath = SceneManager.GetActiveScene().path;

            foreach (var guid in sceneGuids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var scene = EditorSceneManager.OpenScene(path, OpenSceneMode.Single);

                foreach (var root in scene.GetRootGameObjects())
                {
                    foreach (var tmp in root.GetComponentsInChildren<TMP_Text>(true))
                    {
                        AddTmpRow(rows, tmp, $"Scene:{Path.GetFileNameWithoutExtension(path)}");
                    }
                }
            }

            if (!string.IsNullOrEmpty(openScenePath))
            {
                EditorSceneManager.OpenScene(openScenePath, OpenSceneMode.Single);
            }
        }

        private static void CollectFromPrefabs(List<Row> rows)
        {
            var prefabGuids = AssetDatabase.FindAssets("t:Prefab", new[] { "Assets/_DiceBattle" });

            foreach (var guid in prefabGuids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);

                if (prefab == null) continue;

                foreach (var tmp in prefab.GetComponentsInChildren<TMP_Text>(true))
                {
                    AddTmpRow(rows, tmp, $"Prefab:{Path.GetFileNameWithoutExtension(path)}");
                }
            }
        }

        private static void CollectFromConfigs(List<Row> rows)
        {
            var configGuids = AssetDatabase.FindAssets("t:ScriptableObject", new[] { "Assets/_DiceBattle/Data" });

            foreach (var guid in configGuids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var asset = AssetDatabase.LoadAssetAtPath<ScriptableObject>(path);

                if (asset == null) continue;

                var so = new SerializedObject(asset);
                var prop = so.GetIterator();
                var enterChildren = true;

                while (prop.NextVisible(enterChildren))
                {
                    enterChildren = true;

                    if (prop.propertyType != SerializedPropertyType.String) continue;
                    if (string.IsNullOrWhiteSpace(prop.stringValue)) continue;

                    AddRow(rows, prop.stringValue, $"Config:{asset.name}", prop.propertyPath);
                }
            }
        }

        private static void AddTmpRow(List<Row> rows, TMP_Text tmp, string sourcePrefix)
        {
            var localized = tmp.GetComponent<LocalizedTMP>();

            if (localized != null)
            {
                var key = new SerializedObject(localized).FindProperty("_localizationKey").stringValue;

                if (!string.IsNullOrWhiteSpace(key))
                {
                    rows.Add(new Row { Key = key, Source = $"{sourcePrefix}:LocalizedTMP", Text = tmp.text });
                    return;
                }
            }

            AddRow(rows, tmp.text, sourcePrefix, GetHierarchyPath(tmp.transform));
        }

        private static void CollectFromCode(List<Row> rows)
        {
            var constants = CollectConstants();
            var codeFiles = Directory.GetFiles(ScriptsRoot, "*.cs", SearchOption.AllDirectories);

            foreach (var file in codeFiles)
            {
                var relativePath = file.Replace('\\', '/');
                var lines = File.ReadAllLines(file);

                for (var i = 0; i < lines.Length; i++)
                {
                    var line = lines[i];
                    var match = LocalizeCallRegex.Match(line);

                    if (!match.Success) continue;

                    var arg = match.Groups["arg"].Value.Trim();
                    var context = $"{relativePath}:{i + 1}";

                    if (constants.TryGetValue(arg, out var constValue))
                    {
                        rows.Add(new Row { Key = constValue, Source = "Code:Constant", Text = context });
                    }
                    else if (arg.StartsWith("\"") && arg.EndsWith("\""))
                    {
                        var literal = arg.Substring(1, arg.Length - 2);
                        rows.Add(new Row { Key = literal, Source = "Code:Literal", Text = context });
                    }
                    else
                    {
                        Debug.LogWarning($"Localization export: dynamic key '{arg}' at {context}, verify manually.");
                        rows.Add(new Row { Key = $"DYNAMIC:{arg}", Source = "Code:Dynamic", Text = context });
                    }
                }
            }
        }

        /// <summary>
        /// Maps both the short constant name and its fully qualified nested-class path
        /// (e.g. "EndTurn" and "LocKeys.Button.EndTurn") to the literal string value,
        /// so calls like LocalizationManager.Localize(LocKeys.Button.EndTurn) resolve.
        /// </summary>
        private static Dictionary<string, string> CollectConstants()
        {
            var result = new Dictionary<string, string>();
            var codeFiles = Directory.GetFiles(ScriptsRoot, "*.cs", SearchOption.AllDirectories);

            foreach (var file in codeFiles)
            {
                var typeStack = new List<string>();
                var braceOwners = new List<string>();
                var depth = 0;
                string pendingType = null;

                foreach (var rawLine in File.ReadAllLines(file))
                {
                    var line = rawLine;
                    var typeMatch = TypeDeclRegex.Match(line);
                    if (typeMatch.Success)
                    {
                        pendingType = typeMatch.Groups["name"].Value;
                    }

                    var constMatch = ConstDeclRegex.Match(line);
                    if (constMatch.Success)
                    {
                        var name = constMatch.Groups["name"].Value;
                        var value = constMatch.Groups["value"].Value;
                        var qualified = typeStack.Count > 0 ? $"{string.Join(".", typeStack)}.{name}" : name;

                        result[name] = value;
                        result[qualified] = value;
                    }

                    foreach (var ch in line)
                    {
                        if (ch == '{')
                        {
                            depth++;
                            braceOwners.Add(pendingType);
                            if (pendingType != null)
                            {
                                typeStack.Add(pendingType);
                                pendingType = null;
                            }
                        }
                        else if (ch == '}' && depth > 0)
                        {
                            depth--;
                            var owner = braceOwners[braceOwners.Count - 1];
                            braceOwners.RemoveAt(braceOwners.Count - 1);
                            if (owner != null)
                            {
                                typeStack.RemoveAt(typeStack.Count - 1);
                            }
                        }
                    }
                }
            }

            return result;
        }

        private static List<Row> Dedupe(List<Row> rows)
        {
            var seen = new HashSet<(string Key, string Source)>();
            var result = new List<Row>();

            foreach (var row in rows)
            {
                if (seen.Add((row.Key, row.Source)))
                {
                    result.Add(row);
                }
            }

            return result;
        }

        private static void AddRow(List<Row> rows, string text, string source, string context)
        {
            if (string.IsNullOrWhiteSpace(text)) return;

            var key = $"{source}.{context}".Replace(' ', '_');

            rows.Add(new Row { Key = key, Source = source, Text = text });
        }

        private static string GetHierarchyPath(Transform t)
        {
            var names = new List<string>();

            while (t != null)
            {
                names.Insert(0, t.name);
                t = t.parent;
            }

            return string.Join("/", names);
        }

        private static void WriteCsv(string path, List<Row> rows)
        {
            var sb = new StringBuilder();

            sb.AppendLine("Key,Source,Russian,English");

            foreach (var row in rows.OrderBy(r => r.Source).ThenBy(r => r.Key))
            {
                sb.AppendLine($"{Escape(row.Key)},{Escape(row.Source)},{Escape(row.Text)},");
            }

            File.WriteAllText(path, sb.ToString(), Encoding.UTF8);

            string Escape(string value) => "\"" + value.Replace("\"", "\"\"").Replace("\r\n", "\n") + "\"";
        }
    }
}
