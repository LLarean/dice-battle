using System;
using UnityEngine;

namespace DiceBattle.Data
{
    [Serializable]
    public class UnitConfig
    {
        public Sprite[] Portraits = Array.Empty<Sprite>();
        [Header("Health")]
        public int StartHealth = 20;
        public int RegenHealth = 2;
        public int GrowthHealth = 2;
        [Header("Damage")]
        public int StartDamage = 0;
        public int GrowthDamage = 2;
        [Header("Armor")]
        public int StartArmor = 0;
        public int GrowthArmor = 2;
    }
}
