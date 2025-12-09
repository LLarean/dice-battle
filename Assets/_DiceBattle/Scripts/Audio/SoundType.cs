namespace DiceBattle.Audio
{
    /// <summary>
    /// Sound types in the game
    /// </summary>
    public enum SoundType
    {
        DiceGrab,
        DiceShake,
        DiceThrow,
        DieThrow,

        DiceRoll,        // Dice roll
        DiceReroll,      // Dice reroll
        DiceLock,        // Dice lock/unlock
        PlayerAttack,    // Player attack
        PlayerHeal,      // Player healing
        PlayerHit,       // Player takes damage
        EnemyHit,        // Enemy takes damage
        EnemyDefeated,   // Enemy defeated
        EnemySpawn,      // New enemy spawn
        GameOver         // Game Over
    }
}
