namespace DiceBattle.Core
{
    /// <summary>
    /// Represents the different types of dice used in the game.
    /// </summary>
    public enum DiceValue
    {
        /// <summary>
        /// Represents an empty dice type in the game.
        /// </summary>
        Empty = 0,

        /// <summary>
        /// Represents a dice type used to perform offensive actions in the game.
        /// </summary>
        Attack = 1,

        /// <summary>
        /// Represents a dice type focused on defensive actions, such as reducing damage or enhancing protection in the game.
        /// </summary>
        Defense = 2,

        /// <summary>
        /// Represents a dice type used to restore health points in the game.
        /// </summary>
        Heal = 3,
    }
}
