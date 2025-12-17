using DiceBattle.Audio;
using DiceBattle.Screens;
using GameSignals;
using UnityEngine;
using UnityEngine.UI;

namespace DiceBattle
{
    public class TopBar : MonoBehaviour, ITopBarHandler
    {
        [SerializeField] private Transform _panel;
        [Space]
        [SerializeField] private Button _back;
        [SerializeField] private Button _options;

        private RectTransform _canvasRect;

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
            SignalSystem.Subscribe(this);
        }

        private void OnDestroy()
        {
            _back.onClick.RemoveAllListeners();
            _options.onClick.RemoveAllListeners();
            SignalSystem.Unsubscribe(this);
        }

        private void HandleBackClick() => SignalSystem.Raise<IScreenHandler>(handler => handler.Back());

        private void HandleOptionsClick() => SignalSystem.Raise<IOptionsWindowHandler>(handler => handler.Show());
    }
}
