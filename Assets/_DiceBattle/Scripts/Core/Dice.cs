using System;
using System.Linq;
using DiceBattle.Audio;
using DiceBattle.Events;
using DiceBattle.Global;
using DiceBattle.UI;
using GameSignals;
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
        [Header("Empty, Attack, Defense, Heal")]
        [SerializeField] private Sprite[] _faceSprites;
        [Space]
        [SerializeField] private bool _isMenu;

        private Random _random;
        private DiceType _diceType = DiceType.Empty;

        public event Action OnToggled;

        public DiceType DiceType => _diceType;
        public bool IsSelected => _selectionIcon.gameObject.activeSelf;

        public void ResetToEmpty()
        {
            _diceType = DiceType.Empty;
            _faceIcon.sprite = _faceSprites[(int)_diceType];

            ClearSelection();
        }

        public void Roll()
        {
            RewardsData receivedRewards = GameProgress.GetReceivedRewards();
            bool containsDisableEmptyState = receivedRewards.RewardTypes.Contains(RewardType.DisableEmptyState);
            int firstIndex = containsDisableEmptyState ? 1 : 0;

            int randomIndex = _random.Next(firstIndex, _faceSprites.Length);
            _diceType = (DiceType)randomIndex;
            _faceIcon.sprite = _faceSprites[(int)_diceType];

            ClearSelection();
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
    }
}
