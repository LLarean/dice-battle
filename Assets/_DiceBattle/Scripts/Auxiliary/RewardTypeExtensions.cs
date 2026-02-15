using System;

namespace DiceBattle
{
    public static class RewardTypeExtensions
    {
        public static string Title(this DiceType diceType)
        {
            // TODO Translation
            return diceType switch {
                DiceType.Default => "Обычный кубик",

                DiceType.DisableEmptyState => "Совершенный кубик",
                DiceType.AdditionalTry => "Кубик попытки",
                DiceType.AdditionalDice => "Дв",

                DiceType.BaseDamage => "Кубик-меч",
                DiceType.BaseArmor => "Кубик-щит",
                DiceType.DoubleHealth => "Кубик-подорожник",

                DiceType.UpgradeAttack => "Острый кубик",
                DiceType.UpgradeHealth => "Полезный кубик",
                DiceType.UpgradeArmor => "Прочный кубик",

                DiceType.GoldDice => "Золотой кубик (x3)",
                DiceType.SilverDice => "Серебрянный кубик (x2)",

                DiceType.RegenHealth => "Регенерация здоровья",
                DiceType.RestoreHealth => "Восстановить здоровье",
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        public static string Description(this DiceType diceType)
        {
            // TODO Translation
            return diceType switch {
                DiceType.Default => "Свойств не имеет",

                DiceType.DisableEmptyState => "Отключает пустое состояние",
                DiceType.AdditionalTry => "Дают вторую попытку",
                DiceType.AdditionalDice => "Дополнительный кубик",

                DiceType.BaseDamage => "Добавляет базовый урон персонажу",
                DiceType.BaseArmor => "Добавляет базовый армор персонажу",
                DiceType.DoubleHealth => "Удваивает здоровье персонажа",

                DiceType.UpgradeAttack => "Значение атаки х2",
                DiceType.UpgradeHealth => "Значение лечения х2",
                DiceType.UpgradeArmor => "Значение армора х2",

                DiceType.GoldDice => "Золотой кубик (x3)",
                DiceType.SilverDice => "Серебрянный кубик (x2)",

                DiceType.RegenHealth => "Регенерация здоровья",
                DiceType.RestoreHealth => "Восстановить здоровье",
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}
