using UnityEngine;

namespace DiceBattle
{
    public static class DiceTypeColors
    {
        public static Color Get(DiceType type)
        {
            return type switch
            {
                DiceType.Sharp => new Color32(0x8A, 0x2A, 0x2A, 0xFF),
                DiceType.Sturdy => new Color32(0x2B, 0x4A, 0x73, 0xFF),
                DiceType.Healing => new Color32(0x2E, 0x6B, 0x3E, 0xFF),
                DiceType.Reliable => new Color32(0x4A, 0x4A, 0x4A, 0xFF),
                DiceType.Thorns => new Color32(0x2F, 0x5D, 0x6B, 0xFF),
                DiceType.Vampiric => new Color32(0x5E, 0x1A, 0x3A, 0xFF),
                DiceType.Golden => new Color32(0xA8, 0x7B, 0x1E, 0xFF),
                DiceType.Joker => new Color32(0x47, 0x29, 0x5F, 0xFF),
                DiceType.LastStand => new Color32(0x1F, 0x4D, 0x2E, 0xFF),
                DiceType.AdditionalTry => new Color32(0x3E, 0x46, 0x56, 0xFF),
                DiceType.AdditionalDice => new Color32(0x56, 0x48, 0x3A, 0xFF),
                _ => Color.black,
            };
        }
    }
}
