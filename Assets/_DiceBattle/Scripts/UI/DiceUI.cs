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
        [Header("Visual")]
        [SerializeField] private Image _diceImage;
        [SerializeField] private GameObject _lockIndicator;
        
        [Header("Colors")]
        [SerializeField] private Color _normalColor = Color.white;
        [SerializeField] private Color _lockedColor = Color.green;

        private Dice _dice;
        private Button _button;
        private Sprite[] _diceSprites;

        private void Awake()
        {
            _button = GetComponent<Button>();
            _button.onClick.AddListener(OnClicked);
            
            if (_diceImage == null)
                _diceImage = GetComponent<Image>();
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
            if (_dice == null || _diceSprites == null)
                return;

            // Show a sprite of the appropriate type
            int spriteIndex = (int)_dice.CurrentType;
            if (spriteIndex < _diceSprites.Length)
            {
                _diceImage.sprite = _diceSprites[spriteIndex];
            }

            // Change the color depending on the lock
            _diceImage.color = _dice.IsLocked ? _lockedColor : _normalColor;
            
            // Show/hide the lock indicator
            if (_lockIndicator != null)
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