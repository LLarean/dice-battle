using System;
using UnityEngine;

namespace DiceBattle.UI
{
    [Serializable]
    public class UnitData
    {
        public string Title;
        public Sprite Portrait;

        public int MaxHealth;
        public int CurrentHealth;

        public int Damage;
        public int Armor;
    }
}
