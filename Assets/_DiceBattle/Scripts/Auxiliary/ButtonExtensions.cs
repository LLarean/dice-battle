using UnityEngine.UI;

namespace DiceBattle
{
    public static class ButtonExtensions
    {
        private const float _unavailableAlpha = 0.4f;
        private const float _fadeDuration = 0.1f;

        public static void SetAvailable(this Button button, bool isAvailable)
        {
            button.interactable = isAvailable;

            foreach (Graphic graphic in button.GetComponentsInChildren<Graphic>(true))
            {
                graphic.CrossFadeAlpha(isAvailable ? 1f : _unavailableAlpha, _fadeDuration, true);
            }
        }
    }
}
