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
                DiceType.AdditionalDice => "Дополнительный",

                DiceType.BaseDamage => "Кубик-меч",
                DiceType.BaseArmor => "Кубик-щит",
                DiceType.BaseHealth => "Кубик-аптечка",

                DiceType.UpgradeAttack => "Острый кубик",
                DiceType.UpgradeHealth => "Лечебный кубик",
                DiceType.UpgradeArmor => "Прочный кубик",

                DiceType.SilverDice => "Серебрянный кубик (x2)",
                DiceType.GoldDice => "Золотой кубик (x3)",

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
                DiceType.AdditionalTry => "Даёт ещё попытку",
                DiceType.AdditionalDice => "Даёт дополнительный слот",

                DiceType.BaseDamage => "Добавляет базовый урон персонажу",
                DiceType.BaseArmor => "Добавляет базовую броню персонажу",
                DiceType.BaseHealth => "Добавляет базовое здоровье персонажа",

                DiceType.UpgradeAttack => "Значение атаки х2",
                DiceType.UpgradeHealth => "Значение лечения х2",
                DiceType.UpgradeArmor => "Значение брони х2",

                DiceType.SilverDice => "Значение кубиков x2",
                DiceType.GoldDice => "Значение кубиков x3",

                DiceType.RegenHealth => "Регенерация здоровья",
                DiceType.RestoreHealth => "Восстановить здоровье",
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}
