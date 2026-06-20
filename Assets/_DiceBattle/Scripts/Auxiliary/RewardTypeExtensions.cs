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
                DiceType.AdditionalDice => "Доподнительный",

                DiceType.BaseDamage => "Кубик-меч",
                DiceType.BaseArmor => "Кубик-щит",
                DiceType.BaseHealth => "Кубик-подорожник",

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
                DiceType.AdditionalDice => "Даёт дополнительный слот",

                DiceType.BaseDamage => "Добавляет базовый урон персонажу",
                DiceType.BaseArmor => "Добавляет базовую броню персонажу",
                DiceType.BaseHealth => "Добавляет базовое здоровье персонажа",

                DiceType.UpgradeAttack => "Значение атаки х2",
                DiceType.UpgradeHealth => "Значение лечения х2",
                DiceType.UpgradeArmor => "Значение армора х2",

                DiceType.GoldDice => "Значение кубиков x3",
                DiceType.SilverDice => "Значение кубиков x2",

                DiceType.RegenHealth => "Регенерация здоровья",
                DiceType.RestoreHealth => "Восстановить здоровье",
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}
