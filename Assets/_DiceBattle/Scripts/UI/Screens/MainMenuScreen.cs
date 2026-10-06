using System;
using System.Collections.Generic;
using Assets.SimpleLocalization.Scripts;
using DiceBattle.Animations;
using DiceBattle.Audio;
using DiceBattle.Core;
using DiceBattle.Data;
using DiceBattle.Events;
using DiceBattle.Global;
using DiceBattle.Localization;
using GameSignals;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DiceBattle.UI
{
    public class MainMenuScreen : Screen
    {
        [Header("UI References")]
        [SerializeField] private Button _language;
        [Space]
        [SerializeField] private TextMeshProUGUI _title;
        [Space]
        [SerializeField] private Button _options;
        [Space]
        [SerializeField] private Button _start;
        [SerializeField] private TextMeshProUGUI _startLabel;
        [Header("Animation")]
        [SerializeField] private RectTransform _rootUI;
        [SerializeField] private List<Dice> _dice;
        [SerializeField] private RectTransform _rollAnimationArea;
        [SerializeField] private RectTransform _bottomButtons;
        [Header("Config")]
        [SerializeField] private GameConfig _config;

        private const int _maxDiceCount = 6;
        private const float _extraDiceRemoveDuration = 0.2f;

        private GameObjectAnimations _gameObjectAnimations;
        private readonly List<Action> _diceToggleHandlers = new();
        private int _startDiceCount;

        private bool HasSavedBattle => _config.CanSaveBattle && BattleSaveData.HasSavedBattle();

        #region Unity lifecycle

        private void Awake()
        {
            _gameObjectAnimations = new GameObjectAnimations(_rootUI);
            _gameObjectAnimations.SetParams(.2f, .5f, LeanTweenType.easeOutBack);
            _startDiceCount = _dice.Count;
        }

        private void OnEnable()
        {
            _gameObjectAnimations.SlideIn(_title.rectTransform);
            _gameObjectAnimations.SlideIn(_bottomButtons, -1);
            RemoveExtraDice();
            DiceAnimation.Animate(_dice, _rollAnimationArea);

            SetStartLabel();
            LocalizationManager.OnLocalizationChanged += SetStartLabel;

            SignalSystem.Raise<ITopBarHandler>(handler => handler.Hide());
            SignalSystem.Raise<ISoundHandler>(handler => handler.PlayMusic(SoundType.Menu));
        }

        private void Start()
        {
            _language.onClick.AddListener(HandleLanguageClick);
            _options.onClick.AddListener(HandleOptionsClick);
            _start.onClick.AddListener(HandleStartClick);

            foreach (Dice dice in _dice)
            {
                SubscribeToDice(dice);
            }
        }

        private void OnDisable()
        {
            LocalizationManager.OnLocalizationChanged -= SetStartLabel;
        }

        private void OnDestroy()
        {
            _language.onClick.RemoveAllListeners();
            _options.onClick.RemoveAllListeners();
            _start.onClick.RemoveAllListeners();

            for (int i = 0; i < _dice.Count; i++)
            {
                _dice[i].OnToggled -= _diceToggleHandlers[i];
            }

            LeanTween.cancel(gameObject);
        }

        #endregion

        #region Handlers

        private void HandleLanguageClick()
        {
            SystemLanguage nextLanguage = AvailableLanguages.GetNextLanguage(GameSettings.SelectedLanguage);

            LocalizationInitializer.SetLanguage(nextLanguage);
        }

        private void HandleOptionsClick()
        {
            SignalSystem.Raise<IScreenHandler>(handler => handler.ShowWindow(ScreenType.OptionsWindow));
        }

        private void HandleStartClick()
        {
            ScreenType targetScreen = HasSavedBattle ? ScreenType.GameScreen : ScreenType.TavernScreen;

            SignalSystem.Raise<IScreenHandler>(handler => handler.ShowScreen(targetScreen));
            SignalSystem.Raise<ITopBarHandler>(handler => handler.Show());

            if (targetScreen == ScreenType.TavernScreen && GameData.TryGetPendingLootReward(out _))
            {
                SignalSystem.Raise<IScreenHandler>(handler => handler.ShowWindow(ScreenType.LootScreen));
            }
        }

        private void HandleDiceToggled(Dice dice)
        {
            dice.Roll();

            CheckEasterEgg();
        }

        #endregion

        private void SubscribeToDice(Dice dice)
        {
            Action handler = () => HandleDiceToggled(dice);

            _diceToggleHandlers.Add(handler);
            dice.OnToggled += handler;
        }

        // Faces are blank while the dice are in the air, so a mid-roll tap must not count as a match.
        private void CheckEasterEgg()
        {
            if (DiceAnimation.IsRolling)
            {
                return;
            }

            DiceValue firstValue = _dice[0].DiceValue;

            for (int i = 1; i < _dice.Count; i++)
            {
                if (_dice[i].DiceValue != firstValue)
                {
                    return;
                }
            }

            TriggerEasterEgg();
        }

        // Every match adds a die and rethrows them all; a match at the cap goes back to the starting set.
        private void TriggerEasterEgg()
        {
            if (_dice.Count < _maxDiceCount)
            {
                AddExtraDice();
                SignalSystem.Raise<ISoundHandler>(handler => handler.PlaySound(SoundType.DiceThrow));
            }
            else
            {
                RemoveExtraDice();
                SignalSystem.Raise<ISoundHandler>(handler => handler.PlaySound(SoundType.Reward));
            }

            DiceAnimation.Animate(_dice, _rollAnimationArea);
        }

        private void AddExtraDice()
        {
            Dice extraDice = Instantiate(_dice[0], _dice[0].transform.parent);
            var diceRect = (RectTransform)extraDice.transform;

            var corners = new Vector3[4];
            _rootUI.GetWorldCorners(corners);
            float aboveScreen = corners[1].y + diceRect.rect.height * diceRect.lossyScale.y;
            extraDice.transform.position = new Vector3(_rollAnimationArea.position.x, aboveScreen);

            _dice.Add(extraDice);
            SubscribeToDice(extraDice);
        }

        private void RemoveExtraDice()
        {
            for (int i = _dice.Count - 1; i >= _startDiceCount; i--)
            {
                GameObject extraDice = _dice[i].gameObject;

                _dice[i].OnToggled -= _diceToggleHandlers[i];
                _dice[i].DisableButton();
                _dice.RemoveAt(i);
                _diceToggleHandlers.RemoveAt(i);

                LeanTween.cancel(extraDice);
                LeanTween.scale(extraDice, Vector3.zero, _extraDiceRemoveDuration).setEase(LeanTweenType.easeInBack);
                Destroy(extraDice, _extraDiceRemoveDuration);
            }
        }

        private void SetStartLabel()
        {
            string key = HasSavedBattle ? LocKeys.Button.ToBattle : LocKeys.Button.ToTavern;
            _startLabel.text = LocalizationManager.Localize(key);
        }
    }
}
