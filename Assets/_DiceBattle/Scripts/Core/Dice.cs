using System;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

namespace DiceBattle.Core
{
    [RequireComponent(typeof(Image))]
    [RequireComponent(typeof(Button))]
    public class Dice : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private Image _typeIcon;
        [Space]
        [SerializeField] private Image _lockIndicator;
        [Header("Empty, Attack, Defense, Heal")]
        [SerializeField] private Sprite[] _diceSprites;

        private Random _random;
        private DiceType _diceType = DiceType.Empty;

        public event Action OnClicked;
        
        public DiceType DiceType => _diceType;
        public bool IsLocked => _lockIndicator.gameObject.activeSelf;
        
        public void Reset()
        {
            _diceType = DiceType.Empty;
            _typeIcon.sprite = _diceSprites[(int)_diceType];
            Unlock();
        }

        public void Roll()
        {
            var randomIndex = _random.Next(0, _diceSprites.Length);
            _diceType = (DiceType)randomIndex;
            _typeIcon.sprite = _diceSprites[(int)_diceType];
            Unlock();
        }

        public void Unlock()
        {
            _lockIndicator.gameObject.SetActive(false);
            _typeIcon.color = Color.white;
        }

        public void EnableInteractable() => _button.interactable = true;

        public void DisableInteractable() => _button.interactable = false;

        private void Awake()
        {
            _button.onClick.AddListener(OnClick);
        }

        private void Start()
        {
            _random = new Random();
            Reset();
        }

        private void OnDestroy()
        {
            _button.onClick.RemoveAllListeners();
        }

        private void OnClick()
        {
            _lockIndicator.gameObject.SetActive(!_lockIndicator.gameObject.activeSelf);
            _typeIcon.color = _lockIndicator.gameObject.activeSelf ? Color.green : Color.white;
            OnClicked?.Invoke();
            // TODO: SignalSystem.Raise - The cube is locked/unlocked (click sound)
        }
    }
}