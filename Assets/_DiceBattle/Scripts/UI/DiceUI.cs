using UnityEngine;
using UnityEngine.UI;
using DiceBattle.Core;

namespace DiceBattle.UI
{
    /// <summary>
    /// Visual representation of a single dice
    /// </summary>
    [RequireComponent(typeof(Image))]
    [RequireComponent(typeof(Button))]
    public class DiceUI : MonoBehaviour
    {
        [SerializeField] private Image _typeIcon;
        [SerializeField] private GameObject _lockIndicator;
        [Space]
        [SerializeField] private Color _normalColor = Color.white;
        [SerializeField] private Color _lockedColor = Color.green;

        private Button _button;
        private Dice _dice;
        private Sprite[] _diceSprites;

        private void Awake()
        {
            _button = GetComponent<Button>();
            _button.onClick.AddListener(OnClicked);
        }

        public void Initialize(Dice diceModel, Sprite[] sprites)
        {
            _dice = diceModel;
            _diceSprites = sprites;
            UpdateVisual();
        }

        /// <summary>
        /// Update the cube's visual
        /// </summary>
        public void UpdateVisual()
        {
            if (_dice == null || _diceSprites == null) return;

            // Show a sprite of the appropriate type
            int spriteIndex = (int)_dice.CurrentType;
            if (spriteIndex < _diceSprites.Length)
            {
                _typeIcon.sprite = _diceSprites[spriteIndex];
            }

            // Change the color depending on the lock
            _typeIcon.color = _dice.IsLocked ? _lockedColor : _normalColor;
            
            // Show/hide the lock indicator
            _lockIndicator.SetActive(_dice.IsLocked);
        }

        /// <summary>
        /// Enable/disable the click option
        /// </summary>
        public void SetInteractable(bool interactable)
        {
            _button.interactable = interactable;
        }

        /// <summary>
        /// Click on the cube to switch the lock
        /// </summary>
        private void OnClicked()
        {
            if (_dice == null)
                return;

            if (_dice.IsLocked)
                _dice.Unlock();
            else
                _dice.Lock();

            UpdateVisual();
            
            // TODO: SignalSystem.Raise - The cube is locked/unlocked (click sound)
        }
    }
}