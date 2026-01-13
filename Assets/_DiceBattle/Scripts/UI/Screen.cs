using UnityEngine;

namespace DiceBattle
{
    public class Screen : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _canvasGroup;

        public void Show()
        {
            _canvasGroup.alpha = 0;
            gameObject.SetActive(true);
            LeanTween.alphaCanvas(_canvasGroup, 1f, .5f);
        }

        public void Hide()
        {
            _canvasGroup.alpha = 1;

            LeanTween.alphaCanvas(_canvasGroup, 1f, .25f)
                .setOnComplete(() => { gameObject.SetActive(false); });

        }
    }
}
