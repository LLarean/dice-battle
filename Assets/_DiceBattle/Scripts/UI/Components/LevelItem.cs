using System;
using DiceBattle.Audio;
using DiceBattle.Core;
using DiceBattle.Events;
using GameSignals;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DiceBattle.UI
{
    [RequireComponent(typeof(Button))]
    public class LevelItem : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private Image _portrait;
        [SerializeField] private TextMeshProUGUI _label;
        [Space]
        [SerializeField] private Image _aggry;
        [SerializeField] private Image _blackout;

        public event Action OnClicked;

        public void Initialize(LevelData levelData)
        {
            _portrait.sprite = levelData.Portrait;
            _label.text = levelData.Title;

            _button.interactable = levelData.IsAvailable;
            _aggry.gameObject.SetActive(levelData.IsCompleted);
            _blackout.gameObject.SetActive(levelData.IsAvailable == false);
        }

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

        private void OnDestroy() => _button.onClick.RemoveAllListeners();

        private void ClickHandle()
        {
            SignalSystem.Raise<ISoundHandler>(handler => handler.PlaySound(SoundType.Click));
            OnClicked?.Invoke();
        }
    }
}
