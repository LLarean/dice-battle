using DiceBattle.Audio;
using GameSignals;
using UnityEngine;
using UnityEngine.UI;

namespace DiceBattle
{
    public class TopBar : MonoBehaviour, ITopBatHandler
    {
        [SerializeField] private Transform _panel;
        [Space]
        [SerializeField] private Button _back;
        [SerializeField] private Button _options;

        public void Show() => _panel.gameObject.SetActive(true);

        public void Hide() => _panel.gameObject.SetActive(false);

        private void Start()
        {
            _back.onClick.AddListener(HandleBackClick);
            _options.onClick.AddListener(HandleOptionsClick);
        }

        private void OnDestroy()
        {
            _back.onClick.RemoveAllListeners();
            _options.onClick.RemoveAllListeners();
        }

        private void HandleBackClick()
        {
            SignalSystem.Raise<ISoundHandler>(handler => handler.PlaySound(SoundType.Click));
        }

        private void HandleOptionsClick()
        {
            SignalSystem.Raise<ISoundHandler>(handler => handler.PlaySound(SoundType.Click));
            SignalSystem.Raise<IScreenHandler>(handler => handler.ShowScreen(ScreenType.OptionsWindow));
        }
    }
}
