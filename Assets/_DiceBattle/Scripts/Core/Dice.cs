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

        public void ResetToEmpty()
        {
            _diceValue = DiceValue.Empty;
            _faceIcon.sprite = _faceSprites[(int)_diceValue];

            ClearSelection();
        }

        public void Roll()
        {
            DiceList receivedRewards = GameData.GetInventory();
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
        }

        public void Toggle()
        {
            _selectionIcon.gameObject.SetActive(!_selectionIcon.gameObject.activeSelf);
            _image.color = _selectionIcon.gameObject.activeSelf ? Color.yellow : Color.white;
        }

        public void EnableButton() => _button.interactable = true;

        public void DisableButton() => _button.interactable = false;

        private void Start()
        {
            _button.onClick.AddListener(HangleButtonClicked);
            _random = new Random();
            _multiplier.gameObject.SetActive(false);

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
            }
            else
            {
                Roll();
            }

            OnToggled?.Invoke();
        }

        private void ShowMultiplier()
        {
            int multiplier = 1;
            DiceList diceList = GameData.GetInventory();

            if (_diceValue == DiceValue.Attack)
            {
                multiplier += diceList.DiceTypes.Count(r => r == DiceBattle.DiceType.UpgradeAttack);
            }
            else if (_diceValue == DiceValue.Defense)
            {
                multiplier += diceList.DiceTypes.Count(r => r == DiceBattle.DiceType.UpgradeArmor);

            }
            else if (_diceValue == DiceValue.Heal)
            {
                multiplier += diceList.DiceTypes.Count(r => r == DiceBattle.DiceType.UpgradeHealth);
            }

            _multiplier.gameObject.SetActive(multiplier > 1);
            _multiplier.text = "x" + multiplier;
        }

    }
}
