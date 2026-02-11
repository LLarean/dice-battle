using System;

namespace DiceBattle
{
    public static class RewardTypeExtensions
    {
        public static string Title(this DiceType diceType)
        {
            // TODO Translation
            return diceType switch {
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
