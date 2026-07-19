using DiceBattle.Data;
using DiceBattle.Global;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DiceBattle.UI
{
    public class QuestWindow : Screen
    {
        [SerializeField] private GameConfig _gameConfig;
        [Space]
        [SerializeField] private Button _close;
        [SerializeField] private UnitPanel _unitPanel;
        [SerializeField] private TextMeshProUGUI _description;

        private void Start()
        {
            _close.onClick.AddListener(HandleCloseClick);
        }

        private void OnEnable()
        {
            UnitData nextEnemy = _gameConfig.Enemies[GameData.CompletedLevels];
            _unitPanel.SetUnitData(nextEnemy.CloneAtFullHealth());
            _description.text = nextEnemy.Description;
        }

        private void OnDestroy()
        {
            _close.onClick.RemoveAllListeners();
        }

        private void HandleCloseClick()
        {
            gameObject.SetActive(false);
        }
    }
}
