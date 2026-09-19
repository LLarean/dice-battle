using DiceBattle.Core;
using UnityEngine;

namespace DiceBattle
{
    public static class DiceRarityColors
    {
        private static readonly Color _uncommon = new Color32(0x1E, 0xC8, 0x54, 0xFF);
        private static readonly Color _rare = new Color32(0x2E, 0x8F, 0xF7, 0xFF);
        private static readonly Color _legendary = new Color32(0xF7, 0x9E, 0x1E, 0xFF);

        public static Color Get(DiceRarity rarity)
        {
            return rarity switch
            {
                DiceRarity.Uncommon => _uncommon,
                DiceRarity.Rare => _rare,
                DiceRarity.Legendary => _legendary,
                _ => Color.white,
            };
        }
    }
}
