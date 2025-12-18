using System;
using UnityEngine;
using UnityEngine.UI;

namespace DiceBattle
{
    [RequireComponent(typeof(Button))]
    public class LevelItem : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private Image _aggry;
        [SerializeField] private Image _blackout;
        [SerializeField] private int _levelIndex;

        public event Action<int> OnClicked;

        public void EnableAvailable()
        {
            _button.interactable = true;
            _blackout.gameObject.SetActive(false);
            _aggry.gameObject.SetActive(true);
        }

        public void DisableAvailable()
        {
            _button.interactable = false;
            _blackout.gameObject.SetActive(true);
        }

        public void DisableAggry() => _aggry.gameObject.SetActive(false);

        private void Start() => _button.onClick.AddListener(ClickHandle);

        private void ClickHandle() => OnClicked?.Invoke(_levelIndex);

        private void OnDestroy() => _button.onClick.RemoveAllListeners();
    }
}
