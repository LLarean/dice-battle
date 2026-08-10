using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DiceBattle.Localization.Editor
{
    /// <summary>
    /// Собирает весь захардкоженный текст (сцены, префабы, ScriptableObject-конфиги) в один CSV
    /// для последующей загрузки в таблицу локализации. Не находит строки, захардкоженные в коде C#.
    /// </summary>
    public static class LocalizationTextCollector
    {
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

            WriteCsv(path, rows);

            Debug.Log($"Localization export done: {rows.Count} rows -> {path}");
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
                        AddRow(rows, tmp.text, $"Scene:{Path.GetFileNameWithoutExtension(path)}", GetHierarchyPath(tmp.transform));
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
                    AddRow(rows, tmp.text, $"Prefab:{Path.GetFileNameWithoutExtension(path)}", GetHierarchyPath(tmp.transform));
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
