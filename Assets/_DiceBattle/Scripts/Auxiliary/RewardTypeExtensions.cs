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
                DiceType.BaseDamage => DiceIconCategory.Sword,
                DiceType.UpgradeAttack => DiceIconCategory.Sword,

                DiceType.BaseArmor => DiceIconCategory.Shield,
                DiceType.UpgradeArmor => DiceIconCategory.Shield,

                DiceType.BaseHealth => DiceIconCategory.Heart,
                DiceType.UpgradeHealth => DiceIconCategory.Heart,
                DiceType.RegenHealth => DiceIconCategory.Heart,
                DiceType.LastStandDice => DiceIconCategory.Heart,
                DiceType.LifestealDice => DiceIconCategory.Heart,

                _ => DiceIconCategory.Empty,
            };
        }

        public static DiceRarity GetRarity(this DiceType diceType)
        {
            return diceType switch {
                DiceType.LastStandDice => DiceRarity.Legendary,
                DiceType.DisableEmptyState => DiceRarity.Legendary,

                DiceType.GoldDice => DiceRarity.Rare,
                DiceType.LifestealDice => DiceRarity.Rare,

                DiceType.AdditionalTry => DiceRarity.Uncommon,
                DiceType.AdditionalDice => DiceRarity.Uncommon,
                DiceType.SilverDice => DiceRarity.Uncommon,

                _ => DiceRarity.Common,
            };
        }

        public static DiceValue? GetEffectDiceValue(this DiceType diceType)
        {
            return diceType switch {
                DiceType.UpgradeAttack => DiceValue.Attack,
                DiceType.UpgradeArmor => DiceValue.Defense,
                DiceType.UpgradeHealth => DiceValue.Heal,
                _ => null,
            };
        }

        public static string Title(this DiceType diceType)
        {
            return diceType switch {
                DiceType.Default => LocalizationManager.Localize(LocKeys.DiceTitles.Prefix + "default"),

                DiceType.DisableEmptyState => LocalizationManager.Localize(LocKeys.DiceTitles.Prefix + "disable_empty_state"),
                DiceType.AdditionalTry => LocalizationManager.Localize(LocKeys.DiceTitles.Prefix + "additional_try"),
                DiceType.AdditionalDice => LocalizationManager.Localize(LocKeys.DiceTitles.Prefix + "additional_dice"),

                DiceType.BaseDamage => LocalizationManager.Localize(LocKeys.DiceTitles.Prefix + "base_damage"),
                DiceType.BaseArmor => LocalizationManager.Localize(LocKeys.DiceTitles.Prefix + "base_armor"),
                DiceType.BaseHealth => LocalizationManager.Localize(LocKeys.DiceTitles.Prefix + "base_health"),

                DiceType.UpgradeAttack => LocalizationManager.Localize(LocKeys.DiceTitles.Prefix + "upgrade_attack"),
                DiceType.UpgradeHealth => LocalizationManager.Localize(LocKeys.DiceTitles.Prefix + "upgrade_health"),
                DiceType.UpgradeArmor => LocalizationManager.Localize(LocKeys.DiceTitles.Prefix + "upgrade_armor"),

                DiceType.SilverDice => LocalizationManager.Localize(LocKeys.DiceTitles.Prefix + "silver_dice"),
                DiceType.GoldDice => LocalizationManager.Localize(LocKeys.DiceTitles.Prefix + "gold_dice"),

                DiceType.RegenHealth => LocalizationManager.Localize(LocKeys.DiceTitles.Prefix + "regen_health"),
                DiceType.LastStandDice => LocalizationManager.Localize(LocKeys.DiceTitles.Prefix + "last_stand_dice"),
                DiceType.LifestealDice => LocalizationManager.Localize(LocKeys.DiceTitles.Prefix + "lifesteal_dice"),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        public static string Description(this DiceType diceType)
        {
            return diceType switch {
                DiceType.Default => LocalizationManager.Localize(LocKeys.DiceDescriptions.Prefix + "default"),

                DiceType.DisableEmptyState => LocalizationManager.Localize(LocKeys.DiceDescriptions.Prefix + "disable_empty_state"),
                DiceType.AdditionalTry => LocalizationManager.Localize(LocKeys.DiceDescriptions.Prefix + "additional_try"),
                DiceType.AdditionalDice => LocalizationManager.Localize(LocKeys.DiceDescriptions.Prefix + "additional_dice"),

                DiceType.BaseDamage => LocalizationManager.Localize(LocKeys.DiceDescriptions.Prefix + "base_damage"),
                DiceType.BaseArmor => LocalizationManager.Localize(LocKeys.DiceDescriptions.Prefix + "base_armor"),
                DiceType.BaseHealth => LocalizationManager.Localize(LocKeys.DiceDescriptions.Prefix + "base_health"),

                DiceType.UpgradeAttack => LocalizationManager.Localize(LocKeys.DiceDescriptions.Prefix + "upgrade_attack"),
                DiceType.UpgradeHealth => LocalizationManager.Localize(LocKeys.DiceDescriptions.Prefix + "upgrade_health"),
                DiceType.UpgradeArmor => LocalizationManager.Localize(LocKeys.DiceDescriptions.Prefix + "upgrade_armor"),

                DiceType.SilverDice => LocalizationManager.Localize(LocKeys.DiceDescriptions.Prefix + "silver_dice"),
                DiceType.GoldDice => LocalizationManager.Localize(LocKeys.DiceDescriptions.Prefix + "gold_dice"),

                DiceType.RegenHealth => LocalizationManager.Localize(LocKeys.DiceDescriptions.Prefix + "regen_health"),
                DiceType.LastStandDice => LocalizationManager.Localize(LocKeys.DiceDescriptions.Prefix + "last_stand_dice"),
                DiceType.LifestealDice => LocalizationManager.Localize(LocKeys.DiceDescriptions.Prefix + "lifesteal_dice"),
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}
