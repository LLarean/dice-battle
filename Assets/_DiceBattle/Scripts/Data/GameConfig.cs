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
        public UnitConfig Player;
        public UnitConfig Enemy;
        public List<UnitData> Enemies = new();

        [Header("Dice")]
        public int DiceStartCount = 5;
        public int DiceBonusCount = 1;
        public int AttemptsCount = 2;
        public int AttemptsBonusCount = 1;

        [Header("Maximum attempts")]
        [Tooltip("Maximum number of dice rolls")]
        public int MaxAttempts = 3;

        [Header("Debug")]
        #if UNITY_EDITOR
        public bool IsInstaWin = false;
        #else
        public bool IsInstaWin = false;
        #endif
    }
}
