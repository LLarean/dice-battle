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

                _ => DiceIconCategory.Empty,
            };
        }

        public static DiceRarity GetRarity(this DiceType diceType)
        {
            return diceType switch {
                DiceType.LastStand => DiceRarity.Legendary,

                DiceType.Golden => DiceRarity.Rare,
                DiceType.Joker => DiceRarity.Rare,
                DiceType.AdditionalDice => DiceRarity.Rare,

                DiceType.Reliable => DiceRarity.Uncommon,
                DiceType.Thorns => DiceRarity.Uncommon,
                DiceType.Vampiric => DiceRarity.Uncommon,
                DiceType.AdditionalTry => DiceRarity.Uncommon,

                _ => DiceRarity.Common,
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
