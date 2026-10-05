using System;
using System.Linq;
using Assets.SimpleLocalization.Scripts;
using DiceBattle.Animations;
using DiceBattle.Audio;
using DiceBattle.Auxiliary;
using DiceBattle.Events;
using DiceBattle.Localization;
using DiceBattle.UI;
using GameSignals;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

namespace DiceBattle.Core
{
    [RequireComponent(typeof(Button))]
    public class Dice : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private Image _image;
        [Space]
        [SerializeField] private Image _faceIcon;
        [SerializeField] private Image _rarityGlow;
        [SerializeField] private Image _selectionIcon;
        [SerializeField] private TextMeshProUGUI _multiplier;
        [Header("Empty, Attack, Defense, Heal")]
        [SerializeField] private Sprite[] _faceSprites;
        [Space]
        [SerializeField] private bool _isMenu;

        private const float _togglePopScale = 1.15f;
        private const float _togglePopDuration = 0.15f;

        private Random _random;
        private DiceValue _diceValue = DiceValue.Empty;
        private DiceType _type = DiceType.Default;
        private int _boardIndex = -1;

        public event Action OnToggled;

        public DiceValue DiceValue => _diceValue;
        public DiceType Type => _type;
        public bool Interactable => _button.interactable;
        public bool IsSelected => _selectionIcon.gameObject.activeSelf;

        public void SetFixedFace(DiceIconCategory category)
        {
            _faceIcon.sprite = _faceSprites[(int)category];
        }

        public void HideMultiplier() => _multiplier.gameObject.SetActive(false);

        public void ResetToEmpty()
        {
            _diceValue = DiceValue.Empty;
            _faceIcon.sprite = _faceSprites[(int)_diceValue];

            HideMultiplier();
            ClearSelection();
        }

        public void ShowRandomFace()
        {
            int randomIndex = UnityEngine.Random.Range(1, _faceSprites.Length);
            _faceIcon.sprite = _faceSprites[randomIndex];
        }

        public void SetRarityGlow(DiceRarity rarity)
        {
            _rarityGlow.color = DiceRarityColors.Get(rarity);
            _rarityGlow.gameObject.SetActive(rarity == DiceRarity.Legendary);
        }

        public void SetBodyColor(DiceType type) => _faceIcon.color = DiceTypeColors.Get(type);

        public void SetType(DiceType type)
        {
            _type = type;
            SetBodyColor(type);
            SetRarityGlow(type.GetRarity());
        }

        public void SetBoardIndex(int index) => _boardIndex = index;

        public void ShowFixedMultiplier(DiceValue diceValue, int multiplier)
        {
            _multiplier.gameObject.SetActive(multiplier > 1);
            _multiplier.text = GetEffectLabel(diceValue) + "x" + multiplier;
        }

        public void Roll()
        {
            int firstIndex = _type == DiceType.Reliable ? 1 : 0;

            int randomIndex = _random.Next(firstIndex, _faceSprites.Length);
            SetFace(DebugOverrides.TryGetForcedFace(_boardIndex, out DiceValue forcedFace) ? forcedFace : (DiceValue)randomIndex);
            ClearSelection();
        }

        public void SetFace(DiceValue diceValue)
        {
            _diceValue = diceValue;
            _faceIcon.sprite = _faceSprites[(int)_diceValue];
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

        // Not in Start: a restored battle sets the faces right after the dice are created.
        private void Awake()
        {
            _button.onClick.AddListener(HangleButtonClicked);
            _random = new Random();

            if (_isMenu == false)
            {
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
                DiceAnimation.Lift(gameObject, _togglePopScale, _togglePopDuration, 0f);
            }

            OnToggled?.Invoke();
        }

        private void ShowMultiplier()
        {
            if (_isMenu)
            {
                return;
            }

            ShowFixedMultiplier(_diceValue, DiceResult.OwnFaceValue(_type, _diceValue));
        }

        private static string GetEffectLabel(DiceValue diceValue)
        {
            return diceValue switch
            {
                DiceValue.Attack => LocalizationManager.Localize(LocKeys.Dice.AttackAbbr),
                DiceValue.Defense => LocalizationManager.Localize(LocKeys.Dice.DefenseAbbr),
                DiceValue.Heal => LocalizationManager.Localize(LocKeys.Dice.HealAbbr),
                _ => string.Empty,
            };
        }

    }
}
