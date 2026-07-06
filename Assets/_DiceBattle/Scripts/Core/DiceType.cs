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

        GoldDice, // adds +2 to the shared value multiplier
        SilverDice, // adds +1 to the shared value multiplier

        // Restore Health
        RegenHealth,
        RestoreHealth,
    }
}
