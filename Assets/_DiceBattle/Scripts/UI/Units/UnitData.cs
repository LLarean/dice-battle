using System;
using UnityEngine;

namespace DiceBattle.UI
{
    [Serializable]
    public class UnitData
    {
        public string Name;
        [TextArea]
        public string Description = "Этот монстр доставил много хлопот местным жителям.";

        public Sprite Portrait;
        public Sprite Background;

        public int MaxHealth;
        public int CurrentHealth;

        public int Damage;
        public int Armor;
    }
}
