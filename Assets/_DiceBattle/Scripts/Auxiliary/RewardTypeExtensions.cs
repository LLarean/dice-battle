using System;

namespace DiceBattle
{
    public static class RewardTypeExtensions
    {
        public static string Title(this RewardType rewardType)
        {
            // TODO Translation
            return rewardType switch {
                RewardType.DisableEmptyState => "Отключает пустое состояние",
                RewardType.AdditionalTry => "Дают вторую попытку",
                RewardType.AdditionalDice => "Дополнительный кубик",

                RewardType.BaseDamage => "Добавляет базовый урон персонажу",
                RewardType.BaseArmor => "Добавляет базовый армор персонажу",
                RewardType.DoubleHealth => "Удваивает здоровье персонажа",

                RewardType.UpgradeAttack => "Значение атаки х2",
                RewardType.UpgradeHealth => "Значение лечения х2",
                RewardType.UpgradeArmor => "Значение армора х2",

                RewardType.GoldDice => "Золотой кубик (x3)",
                RewardType.SilverDice => "Серебрянный кубик (x2)",

                RewardType.RegenHealth => "Регенерация здоровья",
                RewardType.RestoreHealth => "Восстановить здоровье",
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}
