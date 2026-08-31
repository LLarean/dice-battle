using System.Collections.Generic;
using System.Linq;
using DiceBattle.UI;

namespace DiceBattle.Core
{
    /// <summary>
    /// State and mini-AI for one side of a tournament match.
    /// </summary>
    public class TournamentFighter
    {
        public UnitData Data;
        public int BaseDamage;
        public int BaseArmor;

        public readonly DiceResult Result = new();

        // Greedy bot strategy: keep Attack/Defense/Heal, reroll Empty.
        public List<Dice> SelectRerollTargets(IEnumerable<Dice> dice) =>
            dice.Where(d => d.DiceValue == DiceValue.Empty).ToList();
    }
}
