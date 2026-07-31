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
        private const float _glowSpinDuration = 6f;

        private const float _shakeOffset = 8f;
        private const float _shakeStep = 0.05f;

        private const float _diceBounceHeight = 10f;
        private const float _diceBounceUpDuration = 0.1f;
        private const float _diceBounceDownDuration = 0.2f;
        private const float _diceWobbleAngle = 10f;
        private const float _diceWobbleStep = 0.08f;

        private const float _diceSelectSpinDuration = 0.3f;
        private const float _diceSelectScalePeak = 1.25f;
        private const float _diceSelectScaleUpDuration = 0.15f;
        private const float _diceSelectScaleDownDuration = 0.15f;

        private const float _contentFadeInDuration = 0.2f;
        private const float _contentScalePeak = 1f;
        private const float _contentScaleStart = 0.9f;

        private Item _data;
        private RectTransform _diceRect;
        private Vector2 _diceBasePosition;
        private CanvasGroup _contentGroup;

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

        public void PlaySelectReaction()
        {
            RectTransform rect = ResetDiceTransform();
            GameObject diceObject = rect.gameObject;

            LeanTween.rotateZ(diceObject, 360f, _diceSelectSpinDuration).setEase(LeanTweenType.easeOutQuad);

            LTSeq punch = LeanTween.sequence();
            punch.append(LeanTween.scale(diceObject, Vector3.one * _diceSelectScalePeak, _diceSelectScaleUpDuration).setEase(LeanTweenType.easeOutBack));
            punch.append(LeanTween.scale(diceObject, Vector3.one, _diceSelectScaleDownDuration).setEase(LeanTweenType.easeInOutSine));
        }

        public void PlayDeselectReaction()
        {
            RectTransform rect = ResetDiceTransform();
            GameObject diceObject = rect.gameObject;

            float baseY = _diceBasePosition.y;

            LTSeq bounce = LeanTween.sequence();
            bounce.append(LeanTween.moveY(rect, baseY + _diceBounceHeight, _diceBounceUpDuration).setEase(LeanTweenType.easeOutQuad));
            bounce.append(LeanTween.moveY(rect, baseY, _diceBounceDownDuration).setEase(LeanTweenType.easeOutBounce));

            LTSeq wobble = LeanTween.sequence();
            wobble.append(LeanTween.rotateZ(diceObject, _diceWobbleAngle, _diceWobbleStep).setEase(LeanTweenType.easeInOutSine));
            wobble.append(LeanTween.rotateZ(diceObject, -_diceWobbleAngle, _diceWobbleStep).setEase(LeanTweenType.easeInOutSine));
            wobble.append(LeanTween.rotateZ(diceObject, 0f, _diceWobbleStep).setEase(LeanTweenType.easeInOutSine));
        }

        private RectTransform ResetDiceTransform()
        {
            LeanTween.cancel(_diceRect.gameObject);

            _diceRect.localRotation = Quaternion.identity;
            _diceRect.localScale = Vector3.one;
            _diceRect.anchoredPosition = _diceBasePosition;

            return _diceRect;
        }

        public void SetContentVisible(bool isVisible)
        {
            _dice.gameObject.SetActive(isVisible);
            _title.gameObject.SetActive(isVisible);
            _description.gameObject.SetActive(isVisible);

            bool showGlow = isVisible && _data != null && _data.Type.GetRarity() != DiceRarity.Common;
            _rarityGlow.gameObject.SetActive(showGlow);
        }

        public void SetInteractable(bool isInteractable)
        {
            _button.interactable = isInteractable;
            _contentGroup.blocksRaycasts = isInteractable;
        }

        public void PlayRevealAnimation()
        {
            LeanTween.cancel(gameObject);

            _contentGroup.alpha = 0f;
            var rect = (RectTransform)transform;
            rect.localScale = Vector3.one * _contentScaleStart;

            LeanTween.alphaCanvas(_contentGroup, 1f, _contentFadeInDuration).setEase(LeanTweenType.easeOutQuad);
            LeanTween.scale(gameObject, Vector3.one * _contentScalePeak, _contentFadeInDuration).setEase(LeanTweenType.easeOutBack);
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

            // Guarantee the glow renders behind the dice, otherwise it swallows the select/deselect animation.
            _rarityGlow.transform.SetSiblingIndex(_diceRect.GetSiblingIndex());

            Color faded = _rarityGlow.color;
            faded.a = _pulseMinAlpha;
            _rarityGlow.color = faded;
            _rarityGlow.rectTransform.localRotation = Quaternion.identity;

            LeanTween.alpha(_rarityGlow.rectTransform, _pulseMaxAlpha, _pulseDuration)
                .setEase(LeanTweenType.easeInOutSine)
                .setLoopPingPong(-1);

            LeanTween.rotateZ(_rarityGlow.gameObject, 360f, _glowSpinDuration)
                .setEase(LeanTweenType.linear)
                .setLoopClamp(-1);
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

        public void PlayRejectShake()
        {
            LeanTween.cancel(gameObject);

            var rect = (RectTransform)transform;
            float baseX = rect.anchoredPosition.x;

            LTSeq sequence = LeanTween.sequence();
            sequence.append(LeanTween.moveX(rect, baseX - _shakeOffset, _shakeStep).setEase(LeanTweenType.easeInOutSine));
            sequence.append(LeanTween.moveX(rect, baseX + _shakeOffset, _shakeStep).setEase(LeanTweenType.easeInOutSine));
            sequence.append(LeanTween.moveX(rect, baseX - _shakeOffset * 0.5f, _shakeStep).setEase(LeanTweenType.easeInOutSine));
            sequence.append(LeanTween.moveX(rect, baseX, _shakeStep).setEase(LeanTweenType.easeInOutSine));
        }

        public void RefreshMultiplier()
        {
            DiceValue? effectValue = _data.Type.GetEffectDiceValue();

            if (effectValue.HasValue)
            {
                _dice.ShowFixedMultiplier(effectValue.Value);
            }
        }

        private void Awake()
        {
            _diceRect = (RectTransform)_dice.transform;
            _diceBasePosition = _diceRect.anchoredPosition;

            _contentGroup = GetComponent<CanvasGroup>();
            if (_contentGroup == null)
            {
                _contentGroup = gameObject.AddComponent<CanvasGroup>();
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
            LeanTween.cancel(_dice.gameObject);
            LeanTween.cancel(gameObject);
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
