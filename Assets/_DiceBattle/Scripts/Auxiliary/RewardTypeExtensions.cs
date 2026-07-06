using System;

namespace DiceBattle
{
    public enum DiceIconCategory
    {
        Empty,
        Sword,
        Shield,
        Heart,
    }

    public static class RewardTypeExtensions
    {
        public static DiceIconCategory GetIconCategory(this DiceType diceType)
        {
            return diceType switch {
                DiceType.BaseDamage => DiceIconCategory.Sword,
                DiceType.UpgradeAttack => DiceIconCategory.Sword,

                DiceType.BaseArmor => DiceIconCategory.Shield,
                DiceType.UpgradeArmor => DiceIconCategory.Shield,

                DiceType.BaseHealth => DiceIconCategory.Heart,
                DiceType.UpgradeHealth => DiceIconCategory.Heart,
                DiceType.RegenHealth => DiceIconCategory.Heart,
                DiceType.RestoreHealth => DiceIconCategory.Heart,

                _ => DiceIconCategory.Empty,
            };
        }

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

                DiceType.SilverDice => "Серебряный кубик (+x1)",
                DiceType.GoldDice => "Золотой кубик (+x2)",

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

                DiceType.SilverDice => "Увеличивает общий множитель значений кубиков на 1",
                DiceType.GoldDice => "Увеличивает общий множитель значений кубиков на 2",

                DiceType.RegenHealth => "Регенерация здоровья",
                DiceType.RestoreHealth => "Восстановить здоровье",
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}
