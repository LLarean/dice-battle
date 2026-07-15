using System;
using DiceBattle.Audio;
using DiceBattle.Core;
using DiceBattle.Events;
using GameSignals;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DiceBattle.UI
{
    [RequireComponent(typeof(Button))]
    public class InventoryItem : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [Space]
        [SerializeField] private Dice _dice;
        [SerializeField] private Image _agreeMark;
        [SerializeField] private Image _rarityGlow;
        [SerializeField] private TextMeshProUGUI _title;
        [SerializeField] private TextMeshProUGUI _description;

        private static readonly Color _uncommonColor = new Color32(0x1E, 0xC8, 0x54, 0xFF);
        private static readonly Color _rareColor = new Color32(0x2E, 0x8F, 0xF7, 0xFF);
        private static readonly Color _legendaryColor = new Color32(0xF7, 0x9E, 0x1E, 0xFF);

        private const float _pulseMinAlpha = 0.4f;
        private const float _pulseMaxAlpha = 1f;
        private const float _pulseDuration = 0.8f;

        private Item _data;

        public event Action<DiceType> OnDiceToggled;

        public Item Data => _data;
        public Dice Dice => _dice;

        public void Initialize(Item item)
        {
            _data = item;
            _title.text = item.Type.Title();
            _description.text = item.Type.Description();
            _dice.SetFixedFace(item.Type.GetIconCategory());

            RefreshMultiplier();
            RefreshRarityGlow(item.Type.GetRarity());
        }

        public void SetEquippedStatus(bool isEquipped)
        {
            _agreeMark.gameObject.SetActive(isEquipped);
        }

        public void SetContentVisible(bool isVisible)
        {
            _dice.gameObject.SetActive(isVisible);
            _title.gameObject.SetActive(isVisible);
            _description.gameObject.SetActive(isVisible);

            bool showGlow = isVisible && _data != null && _data.Type.GetRarity() != DiceRarity.Common;
            _rarityGlow.gameObject.SetActive(showGlow);
        }

        public void RefreshRarityGlow(DiceRarity rarity)
        {
            LeanTween.cancel(_rarityGlow.gameObject);

            if (rarity == DiceRarity.Common)
            {
                _rarityGlow.gameObject.SetActive(false);
                return;
            }

            _rarityGlow.color = GetRarityColor(rarity);
            _rarityGlow.gameObject.SetActive(true);

            Color faded = _rarityGlow.color;
            faded.a = _pulseMinAlpha;
            _rarityGlow.color = faded;

            LeanTween.alpha(_rarityGlow.rectTransform, _pulseMaxAlpha, _pulseDuration)
                .setEase(LeanTweenType.easeInOutSine)
                .setLoopPingPong(-1);
        }

        private static Color GetRarityColor(DiceRarity rarity)
        {
            return rarity switch
            {
                DiceRarity.Uncommon => _uncommonColor,
                DiceRarity.Rare => _rareColor,
                DiceRarity.Legendary => _legendaryColor,
                _ => Color.white,
            };
        }

        public void RefreshMultiplier()
        {
            DiceValue? effectValue = _data.Type.GetEffectDiceValue();

            if (effectValue.HasValue)
            {
                _dice.ShowFixedMultiplier(effectValue.Value);
            }
        }

        private void Start()
        {
            _button.onClick.AddListener(HandleButtonClicked);
            _dice.OnToggled += HandleDiceClicked;
        }

        private void OnDestroy()
        {
            _button.onClick.RemoveAllListeners();
            _dice.OnToggled -= HandleDiceClicked;
            LeanTween.cancel(_rarityGlow.gameObject);
        }

        private void HandleButtonClicked()
        {
            OnDiceToggled?.Invoke(_data.Type);
        }

        private void HandleDiceClicked()
        {
            OnDiceToggled?.Invoke(_data.Type);
        }
    }
}
