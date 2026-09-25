using System;
using System.Collections.Generic;
using System.Linq;
using DiceBattle.Data;

namespace DiceBattle.Core
{
    /// <summary>
    /// Tournament progress for the current session: one fight per character class in random order.
    /// Any defeat ends the run; nothing is saved.
    /// </summary>
    public static class TournamentBracket
    {
        private static readonly List<CharacterClass> _opponents = new();

        public static IReadOnlyList<CharacterClass> Opponents => _opponents;
        public static int CurrentIndex { get; private set; }
        public static bool IsDefeated { get; private set; }

        public static bool IsStarted => _opponents.Count > 0;
        public static bool IsCompleted => IsStarted && CurrentIndex >= _opponents.Count;
        public static bool IsFinished => IsDefeated || IsCompleted;
        public static CharacterClass CurrentOpponent => _opponents[CurrentIndex];

        public static void Restart()
        {
            var random = new Random();

            _opponents.Clear();
            _opponents.AddRange(Enum.GetValues(typeof(CharacterClass)).Cast<CharacterClass>().OrderBy(_ => random.Next()));

            CurrentIndex = 0;
            IsDefeated = false;
        }

        public static void RegisterWin() => CurrentIndex++;

        public static void RegisterDefeat() => IsDefeated = true;
    }
}
