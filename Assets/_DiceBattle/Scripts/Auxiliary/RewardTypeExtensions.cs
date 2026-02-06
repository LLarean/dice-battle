using System;

namespace DiceBattle
{
    public static class RewardTypeExtensions
    {
        public static string Title(this RewardType rewardType)
        {
            // TODO Translation
            return rewardType switch {
                RewardType.DisableEmptyState => "Отключить пустое состояние",
                RewardType.AdditionalTry => "Дополнительная попытка",
                RewardType.AdditionalDice => "Дополнительный кубик",

                RewardType.BaseDamage => "Добавить базовый урон",
                RewardType.BaseArmor => "Добавить базовый армор",
                RewardType.DoubleHealth => "Двойное здоровье",

                RewardType.UpgradeAttack => "Золотой кубик атаки",
                RewardType.UpgradeHealth => "Золотой кубик здоровья",
                RewardType.UpgradeArmor => "Золотой кубик армора",

                RewardType.GoldDice => "Золотой кубик (x3)",
                RewardType.SilverDice => "Серебрянный кубик (x2)",
                // RewardType.SilverDice => "Куб дополнительной атаки",
                // RewardType.SilverDice => "Куб дополнительной защиты",
                // RewardType.SilverDice => "Куб дающий реген",
                // RewardType.SilverDice => "Куб отключающий пустые состояние",
                // RewardType.SilverDice => "Куб дающий дополнительную попытку",
                // RewardType.SilverDice => "Куб дающий дополнительную попытку",

                RewardType.RegenHealth => "Регенерация здоровья",
                RewardType.RestoreHealth => "Восстановить здоровье",
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}
