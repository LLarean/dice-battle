using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DiceBattle.UI
{
    public class QuestWindow : Screen
    {
        [Space]
        [SerializeField] private Button _close;
        [SerializeField] private UnitPanel _unitPanel;
        [SerializeField] private TextMeshProUGUI _description;

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
