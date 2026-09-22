using TMPro;
using UnityEngine;

namespace DiceBattle.UI
{
    public static class FloatingText
    {
        public static readonly Color Positive = new(0.30f, 0.82f, 0.22f);
        public static readonly Color Negative = new(0.91f, 0.25f, 0.09f);
        public static readonly Color Critical = new(1f, 0.78f, 0.1f);

        private static readonly Vector2 _size = new(400f, 120f);

        public static TMP_Text Spawn(TMP_Text style, Vector3 worldPosition, string text, Color color, float sizeMultiplier = 1f)
        {
            var go = new GameObject("FloatingText", typeof(RectTransform));
            var rect = (RectTransform)go.transform;
            rect.SetParent(style.canvas.rootCanvas.transform, false);
            rect.sizeDelta = _size;
            rect.position = worldPosition;

            var label = go.AddComponent<TextMeshProUGUI>();
            label.font = style.font;
            label.fontSharedMaterial = style.fontSharedMaterial;
            label.fontSize = style.fontSize * sizeMultiplier;
            label.fontStyle = FontStyles.Bold;
            label.alignment = TextAlignmentOptions.Center;
            label.textWrappingMode = TextWrappingModes.NoWrap;
            label.raycastTarget = false;
            label.color = color;
            label.text = text;

            return label;
        }
    }
}
