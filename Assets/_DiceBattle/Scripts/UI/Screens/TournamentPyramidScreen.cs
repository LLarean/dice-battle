using DiceBattle.Audio;
using DiceBattle.Data;
using DiceBattle.Events;
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

        public void SetContextLabel(string label) => _contextLabel.text = label;

        #region Event handlers

        private void HandleContextClicked() =>
            SignalSystem.Raise<IScreenHandler>(handler => handler.ShowScreen(ScreenType.TournamentScreen));

        #endregion

        #region Unity lifecycle

        private void Start() => _context.onClick.AddListener(HandleContextClicked);

        private void OnDestroy() => _context.onClick.RemoveAllListeners();

        private void OnEnable() => SignalSystem.Raise<ISoundHandler>(handler => handler.PlayMusic(SoundType.Tavern));

        #endregion
    }
}
