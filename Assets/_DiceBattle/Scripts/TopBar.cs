using DiceBattle.Audio;
using DiceBattle.Screens;
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

        private RectTransform _canvasRect;

        [ContextMenu("Show")]
        public void Show()
        {
            _panel.gameObject.SetActive(true);
            
            var gameObjectAnimations = new GameObjectAnimations(_canvasRect);
            gameObjectAnimations.SetParams(.2f, .5f, LeanTweenType.easeOutBack);
            gameObjectAnimations.SlideIn(gameObject.GetComponent<RectTransform>());
        }

        public void Hide() => _panel.gameObject.SetActive(false);

        private void Awake() => _canvasRect = GetComponentInParent<Canvas>().GetComponent<RectTransform>();

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
