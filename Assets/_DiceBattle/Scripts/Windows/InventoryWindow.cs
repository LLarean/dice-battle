using DiceBattle.Audio;
using GameSignals;
using UnityEngine;
using UnityEngine.UI;

namespace DiceBattle.Windows
{
    public class InventoryWindow : MonoBehaviour, IInventoryWindowHandler
    {
        [SerializeField] private Transform _substrate;
        [Space]
        [SerializeField] private Button _close;

        public void Show() => _substrate.gameObject.SetActive(true);

        public void Hide() => _substrate.gameObject.SetActive(false);

        private void Start()
        {
            _close.onClick.AddListener(HandleCloseClick);
            Hide();
            SignalSystem.Subscribe(this);
        }

        private void OnDestroy()
        {
            _close.onClick.RemoveAllListeners();
            SignalSystem.Unsubscribe(this);
        }

        private void HandleCloseClick() => Hide();
    }
}
