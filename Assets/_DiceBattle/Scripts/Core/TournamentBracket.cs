using System.Collections.Generic;
using DiceBattle.Data;

namespace DiceBattle.Core
{
    /// <summary>
    /// Placeholder for a future multi-fight bracket. For now: the queue of
    /// opponents the player faces this tournament.
    /// </summary>
    public class TournamentBracket
    {
        private readonly Queue<CharacterClass> _opponents;

        public TournamentBracket(IEnumerable<CharacterClass> opponents) => _opponents = new Queue<CharacterClass>(opponents);

        public bool HasNext => _opponents.Count > 0;

        public CharacterClass Next() => _opponents.Dequeue();
    }
}
