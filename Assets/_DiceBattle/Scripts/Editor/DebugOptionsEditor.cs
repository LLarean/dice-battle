using System.Collections.Generic;
using System.Reflection;
using NaughtyAttributes;
using NaughtyAttributes.Editor;
using UnityEditor;

namespace DiceBattle.Auxiliary.Editor
{
    [CustomEditor(typeof(DebugOptions))]
    public class DebugOptionsEditor : NaughtyInspector
    {
        private const char _groupSeparator = ':';

        private IEnumerable<MethodInfo> _buttons;

        protected override void OnEnable()
        {
            base.OnEnable();
            _buttons = ReflectionUtility.GetAllMethods(target, method => method.GetCustomAttribute<ButtonAttribute>() != null);
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            string currentGroup = null;
            foreach (MethodInfo button in _buttons)
            {
                string group = GetGroup(button);
                if (group != currentGroup)
                {
                    currentGroup = group;
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField(group, EditorStyles.boldLabel);
                }

                NaughtyEditorGUI.Button(target, button);
            }
        }

        private static string GetGroup(MethodInfo button)
        {
            string text = button.GetCustomAttribute<ButtonAttribute>().Text ?? button.Name;
            int separatorIndex = text.IndexOf(_groupSeparator);
            return separatorIndex < 0 ? text : text[..separatorIndex];
        }
    }
}
