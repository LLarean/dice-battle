using System.Collections.Generic;
using DiceBattle.UI;
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
        [Header("UnitConfigs")]
        public UnitConfig[] PlayerClasses = new UnitConfig[3];
        public List<UnitData> Enemies = new();

        public UnitConfig GetPlayerConfig(CharacterClass characterClass) => PlayerClasses[(int)characterClass];

        [Header("Dice")]
        public List<Item> DefaultInventory = new();
        public int DiceStartCount = 5;

        [Header("Maximum attempts")]
        [Tooltip("Maximum number of dice rolls")]
        public int MaxAttempts = 3;
        [Space]
        public bool CanSaveBattle = true;

        [Header("New Game+")]
        [Tooltip("Enemy stat multiplier per New Game+ cycle. Last value repeats once cycles exceed the array length.")]
        public float[] NewGamePlusMultipliers = { 1.2f, 1.5f, 1.7f, 2f };

        public float GetNewGamePlusMultiplier(int cycle)
        {
            if (cycle <= 0 || NewGamePlusMultipliers.Length == 0)
                return 1f;

            int index = Mathf.Min(cycle - 1, NewGamePlusMultipliers.Length - 1);
            return NewGamePlusMultipliers[index];
        }

        [Header("Debug")]
        #if UNITY_EDITOR
        public bool IsInstaWin = false;
        #else
        public bool IsInstaWin = false;
        #endif
    }
}
