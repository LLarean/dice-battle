using DiceBattle.Core;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

namespace DiceBattle.UI
{
    [RequireComponent(typeof(Image))]
    [RequireComponent(typeof(Button))]
    public class DiceUI : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private Image _typeIcon;
        [SerializeField] private Image _lockIndicator;
        [Space]
        [SerializeField] private Sprite[] _diceSprites;

        private Random _random;
        private DiceType _currentType = DiceType.Empty;

        public DiceType CurrentType => _currentType;
        public bool IsLocked => _lockIndicator.gameObject.activeSelf;

        public void Roll()
        {
            var randomValue = _random.Next(0, 4);
            _currentType = (DiceType)randomValue;

            SetTypeIcon();
            Unlock();
        }

        public void Unlock() => _lockIndicator.gameObject.SetActive(false);
        
        public void Lock() => _lockIndicator.gameObject.SetActive(true);

        public void EnableInteractable() => _button.interactable = true;
        
        public void DisableInteractable() => _button.interactable = false;
        
        private void Awake()
        {
            _button.onClick.AddListener(OnClicked);
        }

        private void Start()
        {
            _random = new Random();
            _typeIcon.sprite = _diceSprites[0];
            _lockIndicator.gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            _button.onClick.RemoveAllListeners();
        }

        private void OnClicked()
        {
            _lockIndicator.gameObject.SetActive(!_lockIndicator.gameObject.activeSelf);
            _typeIcon.color = _lockIndicator.gameObject.activeSelf ? Color.green : Color.white;

            // TODO: SignalSystem.Raise - The cube is locked/unlocked (click sound)
        }

        private void SetTypeIcon()
        {
            var spriteIndex = (int)_currentType;
            
            if (spriteIndex < _diceSprites.Length)
            {
                _typeIcon.sprite = _diceSprites[spriteIndex];
            }
        }
    }
}