using System;
using UnityEngine;

namespace DiceBattle.UI
{
    [Serializable]
    public class UnitData
    {
        // Enemy: localization key. Hero: plain display name (no localization key exists for it).
        public string Name;
        // Localization key, empty for units without a description (e.g. the hero).
        public string Description;

        public Sprite Portrait;
        public Sprite Background;

        public int MaxHealth;
        public int CurrentHealth;

        public int Damage;
        public int Armor;
    }
}
