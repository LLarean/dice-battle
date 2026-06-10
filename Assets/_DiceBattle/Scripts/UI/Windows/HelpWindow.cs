using UnityEngine;
using UnityEngine.UI;

namespace DiceBattle.UI
{
    public class HelpWindow : Screen
    {
        [Space]
        [SerializeField] private Button _close;

        private void Start()
        {
            _close.onClick.AddListener(HandleCloseClick);
        }

        private void OnDestroy()
        {
            _close.onClick.RemoveAllListeners();
        }

        private void HandleCloseClick()
        {
            gameObject.SetActive(false);
        }
    }
}
