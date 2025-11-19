using UnityEngine;

namespace DiceBattle.Core
{
    /// <summary>
    /// Represents an enemy entity with health, attack, defense, and sequential numbering.
    /// </summary>
    public class Enemy
    {
        public int MaxHP { get; private set; }
        public int CurrentHP { get; private set; }
        public int Attack { get; private set; }
        public int Defense { get; private set; }
        public int Number { get; private set; }
        public Sprite Portrait { get; private set; }

        public bool IsAlive => CurrentHP > 0;

        /// <summary>
        /// Creates a new enemy with a given number and calculates its characteristics.
        /// </summary>
        /// <param name="enemyNumber">The sequential number of the enemy, which affects its characteristics.</param>
        /// <returns>The created enemy object.</returns>
        public static Enemy Create(int enemyNumber, Sprite portrait)
        {
            var enemy = new Enemy();
            enemy.Number = enemyNumber;

            enemy.MaxHP = 10 + (enemyNumber - 1) * 2;
            enemy.Attack = 2 + (enemyNumber - 1) / 2;
            enemy.Defense = (enemyNumber - 1) / 3;

            enemy.Portrait = portrait;
            
            enemy.CurrentHP = enemy.MaxHP;

            return enemy;
        }

        /// <summary>
        /// Reduces the enemy's health by applying damage, considering the enemy's defense value.
        /// </summary>
        /// <param name="damage">The amount of incoming damage to be applied to the enemy.</param>
        /// <returns>The actual amount of damage taken by the enemy after considering defense.</returns>
        public int TakeDamage(int damage)
        {
            int actualDamage = Mathf.Max(0, damage - Defense);
            CurrentHP = Mathf.Max(0, CurrentHP - actualDamage);
            
            // TODO: SignalSystem.Raise - The enemy has taken damage (actualDamage)
            
            return actualDamage;
        }

        /// <summary>
        /// Executes an attack by the enemy, returning the attack value.
        /// </summary>
        /// <returns>The attack value of the enemy.</returns>
        public int PerformAttack()
        {
            // TODO: SignalSystem.Raise - The enemy is attacking
            
            return Attack;
        }
    }
}