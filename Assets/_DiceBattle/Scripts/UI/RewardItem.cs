using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DiceBattle
{
    public class RewardItem : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private TextMeshProUGUI _title;

        private RewardType _rewardType;

        public event Action<RewardType> OnClicked;

        public void SetReward(RewardType rewardType)
        {
            _rewardType = rewardType;
            _title.text = GetTitle();
        }

        private void Start() => _button.onClick.AddListener(HandleClick);

        private void OnDestroy() => _button.onClick.RemoveAllListeners();

        private void HandleClick() => OnClicked?.Invoke(_rewardType);

        private string GetTitle()
        {
            // TODO Translation
            return _rewardType switch {
                RewardType.DisableEmptyState => "Отключить пустое состояние",
                RewardType.DisableArmorState => "Отключение стейт армора",
                RewardType.AdditionalTry => "Добавить попытку",
                RewardType.FirstAdditionalDice => "Дополнительный кубик 1",
                RewardType.SecondAdditionalDice => "Дополнительный кубик 1",
                RewardType.AdditionalDamage => "Дополнительный урон",
                RewardType.DoubleDamage => "Двойной урон",
                RewardType.MagicDamage => "Магический урон",
                RewardType.HealthRegen => "Регенерация здоровья",
                RewardType.RestoreHealth => "Восстановить здоровье",
                RewardType.DoubleHealth => "Двойное здоровье",
                RewardType.Armor => "Армор",
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}
