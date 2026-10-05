using UnityEngine;

namespace DiceBattle
{
    public static class DiceTypeColors
    {
        public static Color Get(DiceType type)
        {
            return type switch
            {
                DiceType.Sharp => new Color32(0x8E, 0x1B, 0x1B, 0xFF),
                DiceType.Sturdy => new Color32(0x1F, 0x4E, 0x8C, 0xFF),
                DiceType.Healing => new Color32(0x1F, 0x7A, 0x3A, 0xFF),
                DiceType.Reliable => new Color32(0x5A, 0x5A, 0x5A, 0xFF),
                DiceType.Thorns => new Color32(0x4B, 0x6B, 0x1E, 0xFF),
                DiceType.Vampiric => new Color32(0x5B, 0x0F, 0x2E, 0xFF),
                DiceType.Golden => new Color32(0xC9, 0x96, 0x1A, 0xFF),
                DiceType.Joker => new Color32(0x5B, 0x2A, 0x86, 0xFF),
                DiceType.LastStand => new Color32(0xA3, 0x54, 0x1A, 0xFF),
                DiceType.AdditionalTry => new Color32(0x1B, 0x7F, 0x7F, 0xFF),
                DiceType.AdditionalDice => new Color32(0x7A, 0x5C, 0x3A, 0xFF),
                _ => Color.black,
            };
        }
    }
}
