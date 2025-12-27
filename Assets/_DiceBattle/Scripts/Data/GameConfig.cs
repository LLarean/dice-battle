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
        [Tooltip("The player's initial health")]
        public int PlayerStartHealth = 20;
        [Tooltip("The player's initial armor")]
        public int PlayerStartArmor = 0;
        [Tooltip("Armor boost for each armor reward")]
        public int ArmorBonus = 2;
        [Tooltip("Attack boost for each attack reward")]
        public int AttackBonus = 2;
        [Tooltip("")]
        public Sprite PlayerPortrait;

        [Header("Dices")]
        [Tooltip("Number of dices")]
        public int DiceCount = 5;

        [Header("Enemies - Basic Stats (Enemy #1)")]
        [Tooltip("The base health of the first enemy")]
        public int EnemyBaseHealth = 10;

        [Tooltip("The basic attack of the first enemy")]
        public int EnemyBaseAttack = 2;

        [Tooltip("Basic defense of the first enemy")]
        public int EnemyBaseDefense = 0;

        [Header("Enemies - Increased Stats")]
        [Tooltip("HP boost for each enemy")]
        public int EnemyHPGrowth = 2;

        [Tooltip("Attack boost for every N enemies")]
        public int EnemyAttackGrowthRate = 2; // +1 ATK Every 2 enemies

        [Tooltip("Defense boost for every N enemies")]
        public int EnemyDefenseGrowthRate = 3; // +1 DEF Every 3 enemies

        [Header("Enemies")]
        [Tooltip("Enemy Sprites")]
        public Sprite[] EnemiesPortraits = new Sprite[4];

        [Header("Maximum attempts")]
        [Tooltip("Maximum number of dice rolls")]
        public int MaxAttempts = 3;


    }
}
