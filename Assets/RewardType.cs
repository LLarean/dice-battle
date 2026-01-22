using DiceBattle.UI;

namespace DiceBattle
{
    /// <summary>
    /// This file must be updated when updating -
    /// <seealso cref="RewardTypeExtensions"/>>
    /// </summary>
    public enum RewardType
    {
        // General
        DisableEmptyState,
        AdditionalTry,
        AdditionalDice,

        // Character Upgrades
        BaseDamage,
        BaseArmor,
        DoubleHealth,

        // Dice Upgrades
        UpgradeAttack,
        UpgradeHealth,
        UpgradeArmor,

        // Restore Health
        RegenHealth,
        RestoreHealth,
    }
}
