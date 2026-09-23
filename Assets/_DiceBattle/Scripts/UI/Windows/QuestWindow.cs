using Assets.SimpleLocalization.Scripts;
using DiceBattle.Data;
using DiceBattle.Events;
using DiceBattle.Global;
using GameSignals;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DiceBattle.UI
{
    public class QuestWindow : Screen
    {
        [Space]
        [SerializeField] private Button _close;
        [SerializeField] private UnitPanel _unitPanel;
        [SerializeField] private TextMeshProUGUI _description;
        [Space]
        [SerializeField] private GameConfig _gameConfig;

        private void Start()
        {
            _close.onClick.AddListener(HandleCloseClick);
        }

        private void OnEnable()
        {
            // After a full clear the next run starts over from the first enemy.
            UnitData nextEnemy = _gameConfig.Enemies[GameData.CompletedLevels % _gameConfig.Enemies.Count];
            _unitPanel.SetUnitData(nextEnemy.CloneAtFullHealth());
            _description.text = LocalizationManager.Localize(nextEnemy.Description);
        }

        private void OnDestroy()
        {
            _close.onClick.RemoveAllListeners();
        }

        private void HandleCloseClick()
        {
            SignalSystem.Raise<IScreenHandler>(handler => handler.CloseTopWindow());
        }
    }
}
