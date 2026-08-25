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
            UnitData nextEnemy = _gameConfig.Enemies[GameData.CompletedLevels];
            _unitPanel.SetUnitData(nextEnemy.CloneAtFullHealth());
            string descriptionText = LocalizationManager.Localize(nextEnemy.Description);
            // _description.text = descriptionText;
            // TODO Localization
            _description.text = nextEnemy.Description;
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
