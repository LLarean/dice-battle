namespace DiceBattle
{
    /// <summary>
    /// This file must be updated when updating -
    /// <seealso cref="RewardTypeExtensions"/>>
    /// </summary>
    public enum RewardType
    {
        //Everything is on the dice. A cube can give additional armor,
        //a cube can give an attack, and so on.

        // General
        DisableEmptyState,
        AdditionalTry, // need specific dice
        AdditionalDice,

        // Character Upgrades
        BaseDamage,
        BaseArmor,
        DoubleHealth,

        // Dice Upgrades
        UpgradeAttack,
        UpgradeHealth,
        UpgradeArmor,

        GoldDice, // value x3
        SilverDice, // value x2

        // Restore Health
        RegenHealth,
        RestoreHealth,
    }
}
