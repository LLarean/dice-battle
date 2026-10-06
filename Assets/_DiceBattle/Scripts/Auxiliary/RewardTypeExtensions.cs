using System;
using Assets.SimpleLocalization.Scripts;
using DiceBattle.Core;
using DiceBattle.Localization;

namespace DiceBattle
{
    public enum DiceIconCategory
    {
        Empty,
        Sword,
        Shield,
        Heart,
        Lock,
        Crown,
        Question,
        Reroll,
        ExtraDice,
    }

    public static class RewardTypeExtensions
    {
        public static DiceIconCategory GetIconCategory(this DiceType diceType)
        {
            return diceType switch {
                DiceType.Sharp => DiceIconCategory.Sword,
                DiceType.Vampiric => DiceIconCategory.Sword,

                DiceType.Sturdy => DiceIconCategory.Shield,
                DiceType.Thorns => DiceIconCategory.Shield,

                DiceType.Healing => DiceIconCategory.Heart,
                DiceType.LastStand => DiceIconCategory.Heart,

                DiceType.Reliable => DiceIconCategory.Lock,
                DiceType.Golden => DiceIconCategory.Crown,
                DiceType.Joker => DiceIconCategory.Question,
                DiceType.AdditionalTry => DiceIconCategory.Reroll,
                DiceType.AdditionalDice => DiceIconCategory.ExtraDice,

                _ => DiceIconCategory.Empty,
            };
        }

        public static DiceRarity GetRarity(this DiceType diceType)
        {
            return diceType switch {
                DiceType.AdditionalDice => DiceRarity.Legendary,

                DiceType.Golden => DiceRarity.Rare,
                DiceType.AdditionalTry => DiceRarity.Rare,
                DiceType.LastStand => DiceRarity.Rare,

                DiceType.Thorns => DiceRarity.Uncommon,
                DiceType.Vampiric => DiceRarity.Uncommon,
                DiceType.Joker => DiceRarity.Uncommon,

                _ => DiceRarity.Common,
            };
        }

        public static int GetDropWeight(this DiceRarity rarity)
        {
            return rarity switch {
                DiceRarity.Uncommon => 6,
                DiceRarity.Rare => 3,
                DiceRarity.Legendary => 2,
                _ => 10,
            };
        }

        public static DiceValue? GetEffectDiceValue(this DiceType diceType)
        {
            return diceType switch {
                DiceType.Sharp => DiceValue.Attack,
                DiceType.Sturdy => DiceValue.Defense,
                DiceType.Healing => DiceValue.Heal,
                _ => null,
            };
        }

        public static string Title(this DiceType diceType) =>
            LocalizationManager.Localize(LocKeys.DiceTitles.Prefix + diceType.LocKey());

        public static string Description(this DiceType diceType) =>
            LocalizationManager.Localize(LocKeys.DiceDescriptions.Prefix + diceType.LocKey());

        private static string LocKey(this DiceType diceType)
        {
            return diceType switch {
                DiceType.Default => "default",

                DiceType.Sharp => "sharp",
                DiceType.Sturdy => "sturdy",
                DiceType.Healing => "healing",

                DiceType.Reliable => "reliable",
                DiceType.Thorns => "thorns",
                DiceType.Vampiric => "vampiric",
                DiceType.Golden => "golden",
                DiceType.Joker => "joker",

                DiceType.LastStand => "last_stand",
                DiceType.AdditionalTry => "additional_try",
                DiceType.AdditionalDice => "additional_dice",
                _ => throw new ArgumentOutOfRangeException(nameof(diceType), diceType, null),
            };
        }
    }
}
