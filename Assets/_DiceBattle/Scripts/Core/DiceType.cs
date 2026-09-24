namespace DiceBattle
{
    /// <summary>
    /// This file must be updated when updating -
    /// <seealso cref="RewardTypeExtensions"/>>
    /// </summary>
    public enum DiceType
    {
        // Each equipped die carries its own effect, triggered by its own face.

        Default,

        // Face value upgrades
        Sharp, // Attack = 2
        Sturdy, // Defense = 2
        Healing, // Heal = 2

        // Face tricks
        Reliable, // never rolls Empty
        Thorns, // Defense also deals 1 damage
        Vampiric, // Attack also heals 1
        Golden, // +1 to every die showing the same face
        Joker, // Empty turns into the most common face among the other dice

        // Passive, the die itself rolls as a default one
        LastStand, // survives one lethal hit with 1 HP, once per battle
        AdditionalTry, // +1 reroll
        AdditionalDice, // +1 slot, takes no slot itself
    }

    public enum DiceRarity
    {
        Common,
        Uncommon,
        Rare,
        Legendary,
    }
}
