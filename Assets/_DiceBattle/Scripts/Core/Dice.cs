using System;
using System.Linq;
using DiceBattle.Audio;
using DiceBattle.Events;
using DiceBattle.Global;
using DiceBattle.UI;
using GameSignals;
using TMPro;
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
        [SerializeField] private Image _image;
        [Space]
        [SerializeField] private Image _faceIcon;
        [SerializeField] private Image _selectionIcon;
        [SerializeField] private TextMeshProUGUI _multiplier;
        [Header("Empty, Attack, Defense, Heal")]
        [SerializeField] private Sprite[] _faceSprites;
        [Space]
        [SerializeField] private bool _isMenu;

        private Random _random;
        private DiceValue _diceValue = DiceValue.Empty;

        public event Action OnToggled;

        public DiceValue DiceValue => _diceValue;
        public bool IsSelected => _selectionIcon.gameObject.activeSelf;

        public void SetFixedFace(DiceIconCategory category)
        {
            _faceIcon.sprite = _faceSprites[(int)category];
        }

        public void ResetToEmpty()
        {
            _diceValue = DiceValue.Empty;
            _faceIcon.sprite = _faceSprites[(int)_diceValue];

            ClearSelection();
        }

        public void ShowRandomFace()
        {
            int randomIndex = UnityEngine.Random.Range(1, _faceSprites.Length);
            _faceIcon.sprite = _faceSprites[randomIndex];
        }

        public void ShowFixedMultiplier(DiceValue diceValue)
        {
            DiceList diceList = GameData.GetEquippedAsDiceList();
            int multiplier = DiceResult.CalculateSingle(diceValue, diceList);

            _multiplier.gameObject.SetActive(multiplier > 1);
            _multiplier.text = GetEffectLabel(diceValue) + "x" + multiplier;
        }

        public void Roll()
        {
            DiceList receivedRewards = GameData.GetEquippedAsDiceList();
            bool containsDisableEmptyState = receivedRewards.DiceTypes.Contains(DiceType.DisableEmptyState);
            int firstIndex = containsDisableEmptyState ? 1 : 0;

            int randomIndex = _random.Next(firstIndex, _faceSprites.Length);
            _diceValue = (DiceValue)randomIndex;
            _faceIcon.sprite = _faceSprites[(int)_diceValue];

            ClearSelection();
            ShowMultiplier();
        }

        public void ClearSelection()
        {
            _selectionIcon.gameObject.SetActive(false);
            _image.color = Color.white;
            _multiplier.color = Color.white;
        }

        public void Toggle()
        {
            _selectionIcon.gameObject.SetActive(!_selectionIcon.gameObject.activeSelf);
            _image.color = _selectionIcon.gameObject.activeSelf ? Color.yellow : Color.white;
            _multiplier.color = _selectionIcon.gameObject.activeSelf ? Color.yellow : Color.white;
        }

        public void SetSelection(bool isSelected)
        {
            _selectionIcon.gameObject.SetActive(isSelected);
            _image.color = _selectionIcon.gameObject.activeSelf ? Color.yellow : Color.white;
            _multiplier.color = _selectionIcon.gameObject.activeSelf ? Color.yellow : Color.white;
        }

        public void EnableButton()
        {
            _button.interactable = true;
            _multiplier.color = Color.white;
        }

        public void DisableButton()
        {
            _button.interactable = false;
            _multiplier.color = Color.gray;
        }

        private void Start()
        {
            _button.onClick.AddListener(HangleButtonClicked);
            _random = new Random();

            if (_isMenu == false)
            {
                _multiplier.gameObject.SetActive(false);
                ResetToEmpty();
            }
        }

        private void OnDestroy()
        {
            _button.onClick.RemoveAllListeners();
        }

        private void HangleButtonClicked()
        {
            SignalSystem.Raise<ISoundHandler>(handler => handler.PlaySound(SoundType.DiceGrab));

            if (_isMenu == false)
            {
                Toggle();
            }

            OnToggled?.Invoke();
        }

        private void ShowMultiplier()
        {
            if (_isMenu)
            {
                return;
            }

            ShowFixedMultiplier(_diceValue);
        }

        private static string GetEffectLabel(DiceValue diceValue)
        {
            return diceValue switch
            {
                DiceValue.Attack => "УР ",
                DiceValue.Defense => "ЗЩ ",
                DiceValue.Heal => "ХП ",
                _ => string.Empty,
            };
        }

    }
}
