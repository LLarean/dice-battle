using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace DiceBattle.Screens
{
    public class GameScreen : MonoBehaviour
    {
        [Header("Panels")]
        [SerializeField] private UnitPanel _player;
        [SerializeField] private UnitPanel _enemy;
        [Space]
        [SerializeField] private DicePanel _dicePanel;
        [Space]
        [SerializeField] private Button _context;
        [SerializeField] private TextMeshProUGUI _contextLabel;

        public event Action OnContextClicked;
        public event Action OnDiceClicked;

        public void Initialize(int maxHealth)
        {
            _player.HideAllStats();
            _player.SetMaxHealth(maxHealth);
        }

        public void SetContextLabel(string label) => _contextLabel.text = label;

        public void UpdateHealth(int currentHealth) => _player.UpdateHealth(currentHealth);

        public void UpdatePlayerDefense(int defense) => _player.UpdateDefense(defense);

        private void Start()
        {
            _context.onClick.AddListener(ContextClick);
            _dicePanel.OnDiceClicked += DiceClick;
        }

        private void OnDestroy()
        {
            _context.onClick.RemoveAllListeners();
            _dicePanel.OnDiceClicked -= DiceClick;
        }

        private void ContextClick() => OnContextClicked?.Invoke();

        private void DiceClick() => OnDiceClicked?.Invoke();
    }
}