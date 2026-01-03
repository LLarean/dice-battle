using UnityEngine;

namespace DiceBattle.Data
{
    /// <summary>
    /// Game Balance Settings (ScriptableObject)
    /// To create: Assets -> Create -> Dice Battle -> Game Config
    /// </summary>
    [CreateAssetMenu(fileName = "GameConfig", menuName = "Dice Battle/Game Config", order = 0)]
    public class GameConfig : ScriptableObject
    {
        [Header("Player")]
        public Sprite PlayerPortrait;
        public UnitConfig Player;

        [Header("Enemies")]
        public Sprite[] EnemiesPortraits = new Sprite[4];
        public UnitConfig Enemy;

        [Header("Dices")]
        public int DiceStartCount = 5;
        public int DiceBonusCount = 1;
        public int AttemptsCount = 2;
        public int AttemptsBonusCount = 1;

        [Header("Maximum attempts")]
        [Tooltip("Maximum number of dice rolls")]
        public int MaxAttempts = 3;
    }
}
