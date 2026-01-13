using DiceBattle.Animations;
using DiceBattle.Events;
using DiceBattle.UI;
using GameSignals;
using UnityEngine;
using UnityEngine.UI;

namespace DiceBattle.UI
{
    public class TopBar : MonoBehaviour, ITopBarHandler
    {
        [SerializeField] private RectTransform _rootUI;
        [SerializeField] private Transform _panel;
        [Space]
        [SerializeField] private Button _back;
        [SerializeField] private Button _options;

        private GameObjectAnimations _gameObjectAnimations;

        public void Show()
        {
            _panel.gameObject.SetActive(true);

            // _gameObjectAnimations.SetParams(.2f, .5f, LeanTweenType.easeOutBack);
            // _gameObjectAnimations.SlideIn(gameObject.GetComponent<RectTransform>());
        }

        public void Hide() => _panel.gameObject.SetActive(false);

        private void Awake() => _gameObjectAnimations = new GameObjectAnimations(_rootUI);

        private void Start()
        {
            _back.onClick.AddListener(HandleBackClick);
            _options.onClick.AddListener(HandleOptionsClick);

            SignalSystem.Subscribe(this);
        }

        private void OnDestroy()
        {
            _back.onClick.RemoveAllListeners();
            _options.onClick.RemoveAllListeners();

            SignalSystem.Unsubscribe(this);
        }

        private void HandleBackClick() => SignalSystem.Raise<IScreenHandler>(handler => handler.Back());

        private void HandleOptionsClick() => SignalSystem.Raise<IScreenHandler>(handler => handler.ShowWindow(ScreenType.OptionsWindow));
    }
}
