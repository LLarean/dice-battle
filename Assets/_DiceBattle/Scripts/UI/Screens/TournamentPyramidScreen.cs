using System.Collections.Generic;
using Assets.SimpleLocalization.Scripts;
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
    public class TournamentPyramidScreen : Screen
    {
        [SerializeField] private GameConfig _config;
        [Space]
        [SerializeField] private TournamentLevel _tournamentLevel;
        [SerializeField] private Transform _levelSpawn;
        [Space]
        [SerializeField] private Button _context;
        [SerializeField] private TextMeshProUGUI _contextLabel;

        private readonly List<TournamentLevel> _levels = new();

        public void SetContextLabel(string label) => _contextLabel.text = label;

        private void Refresh()
        {
            if (_levels.Count == 0)
            {
                SpawnLevels();
            }

            UnitData player = _config.GetPlayerConfig(GameData.SelectedCharacterClass).CreateUnitData();

            for (int i = 0; i < _levels.Count; i++)
            {
                _levels[i].SetUnits(player, _config.GetPlayerConfig(TournamentBracket.Opponents[i]).CreateUnitData());
                SetLevelState(_levels[i], i);
            }

            string key = TournamentBracket.IsFinished ? LocKeys.Button.Again : LocKeys.Button.ToBattle;
            SetContextLabel(LocalizationManager.Localize(key));
        }

        private static void SetLevelState(TournamentLevel level, int index)
        {
            if (index < TournamentBracket.CurrentIndex)
            {
                level.SetPassedState();
            }
            else if (index > TournamentBracket.CurrentIndex)
            {
                level.SetLockedState(index + 1);
            }
            else if (TournamentBracket.IsDefeated)
            {
                level.SetDefeatedState();
            }
            else
            {
                level.SetCurrentState();
            }
        }

        // The first fight sits at the bottom of the pyramid, the final one at the top.
        private void SpawnLevels()
        {
            foreach (Transform child in _levelSpawn)
            {
                Destroy(child.gameObject);
            }

            for (int i = 0; i < TournamentBracket.Opponents.Count; i++)
            {
                TournamentLevel level = Instantiate(_tournamentLevel, _levelSpawn);
                level.transform.SetAsFirstSibling();
                _levels.Add(level);
            }
        }

        #region Event handlers

        private void HandleContextClicked()
        {
            if (TournamentBracket.IsFinished)
            {
                TournamentBracket.Restart();
                Refresh();
                return;
            }

            SignalSystem.Raise<IScreenHandler>(handler => handler.ShowScreen(ScreenType.TournamentScreen));
        }

        #endregion

        #region Unity lifecycle

        private void Start() => _context.onClick.AddListener(HandleContextClicked);

        private void OnDestroy() => _context.onClick.RemoveAllListeners();

        private void OnEnable()
        {
            if (TournamentBracket.IsStarted == false)
            {
                TournamentBracket.Restart();
            }

            Refresh();
            SignalSystem.Raise<ISoundHandler>(handler => handler.PlayMusic(SoundType.Tavern));
        }

        #endregion
    }
}
