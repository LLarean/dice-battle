using UnityEngine;
using UnityEngine.UI;

namespace DiceBattle.UI
{
    // ScrollRect makes room for the scrollbar only in a layout pass of its own,
    // and a change of the content's size (new text, a list filled in) does not start one.
    [RequireComponent(typeof(ScrollRect))]
    public class ScrollViewportFit : MonoBehaviour
    {
        private ScrollRect _scrollRect;
        private Vector2 _contentSize;

        private void Awake() => _scrollRect = GetComponent<ScrollRect>();

        private void LateUpdate()
        {
            Vector2 contentSize = _scrollRect.content.rect.size;

            if (contentSize == _contentSize)
            {
                return;
            }

            _contentSize = contentSize;
            LayoutRebuilder.MarkLayoutForRebuild((RectTransform)transform);
        }
    }
}
