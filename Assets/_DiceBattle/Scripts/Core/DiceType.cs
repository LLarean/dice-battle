namespace DiceBattle
{
    /// <summary>
    /// This file must be updated when updating -
    /// <seealso cref="RewardTypeExtensions"/>>
    /// </summary>
    public enum DiceType
    {
        //Everything is on the dice. A dice can give additional armor,
        //a dice can give an attack, and so on.

        Default,

        // General
        DisableEmptyState,
        AdditionalTry, // need specific dice
        AdditionalDice,

        // Character Upgrades
        BaseDamage,
        BaseArmor,
        BaseHealth,

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
