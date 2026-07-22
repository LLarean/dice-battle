using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DiceBattle.UI
{
    public class ConfirmWindow : Screen
    {
        [Space]
        [SerializeField] private TextMeshProUGUI _title;
        [SerializeField] private TextMeshProUGUI _message;
        [Space]
        [SerializeField] private Button _accept;
        [SerializeField] private Button _cancel;

        private void Start()
        {
            _accept.onClick.AddListener(HandleCloseClick);
            _cancel.onClick.AddListener(HandleCloseClick);
        }

        private void OnDestroy()
        {
            _accept.onClick.RemoveAllListeners();
            _cancel.onClick.RemoveAllListeners();
        }

        private void HandleCloseClick()
        {
            gameObject.SetActive(false);
        }
    }
}
