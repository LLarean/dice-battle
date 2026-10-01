using System;
using System.Collections.Generic;
using System.Linq;
using DiceBattle.Data;
using DiceBattle.Global;
using UnityEngine;

namespace DiceBattle.Core
{
    /// <summary>
    /// Tournament progress: one fight per character class in random order.
    /// Any defeat ends the run; closing the game mid-match counts as a defeat.
    /// </summary>
    public static class TournamentBracket
    {
        [Serializable]
        private class Snapshot
        {
            public List<CharacterClass> Opponents;
            public int CurrentIndex;
            public bool IsDefeated;
            public bool IsMatchInProgress;
        }

        private static readonly List<CharacterClass> _opponents = new();

        public static IReadOnlyList<CharacterClass> Opponents => _opponents;
        public static int CurrentIndex { get; private set; }
        public static bool IsDefeated { get; private set; }

        public static bool IsStarted => _opponents.Count > 0;
        public static bool IsCompleted => IsStarted && CurrentIndex >= _opponents.Count;
        public static bool IsFinished => IsDefeated || IsCompleted;
        public static CharacterClass CurrentOpponent => _opponents[CurrentIndex];

        static TournamentBracket() => Load();

        public static void Restart()
        {
            var random = new System.Random();

            _opponents.Clear();
            _opponents.AddRange(Enum.GetValues(typeof(CharacterClass)).Cast<CharacterClass>().OrderBy(_ => random.Next()));

            CurrentIndex = 0;
            IsDefeated = false;
            Save();
        }

        public static void BeginMatch() => Save(isMatchInProgress: true);

        public static void RegisterWin()
        {
            CurrentIndex++;
            Save();

            if (IsCompleted)
            {
                GameData.PendingInnkeeperEvent = InnkeeperEvent.TournamentWon;
            }
        }

        public static void RegisterDefeat()
        {
            IsDefeated = true;
            Save();
            GameData.PendingInnkeeperEvent = InnkeeperEvent.TournamentLost;
        }

        public static void Clear()
        {
            _opponents.Clear();
            CurrentIndex = 0;
            IsDefeated = false;
            PlayerPrefs.DeleteKey(PlayerPrefsKeys.TournamentState);
        }

#if UNITY_EDITOR
        public static void DebugSetOpponents(IEnumerable<CharacterClass> opponents)
        {
            _opponents.Clear();
            _opponents.AddRange(opponents);
            CurrentIndex = 0;
            IsDefeated = false;
            Save();
        }

        public static void DebugSkipToFinal()
        {
            if (IsStarted == false)
            {
                Restart();
            }

            CurrentIndex = _opponents.Count - 1;
            IsDefeated = false;
            Save();
        }
#endif

        private static void Save(bool isMatchInProgress = false)
        {
            var snapshot = new Snapshot
            {
                Opponents = _opponents,
                CurrentIndex = CurrentIndex,
                IsDefeated = IsDefeated,
                IsMatchInProgress = isMatchInProgress,
            };

            PlayerPrefs.SetString(PlayerPrefsKeys.TournamentState, JsonUtility.ToJson(snapshot));
            PlayerPrefs.Save();
        }

        private static void Load()
        {
            string json = PlayerPrefs.GetString(PlayerPrefsKeys.TournamentState, null);

            if (string.IsNullOrEmpty(json))
            {
                return;
            }

            Snapshot snapshot = JsonUtility.FromJson<Snapshot>(json);

            _opponents.AddRange(snapshot.Opponents);
            CurrentIndex = snapshot.CurrentIndex;
            IsDefeated = snapshot.IsDefeated || snapshot.IsMatchInProgress;
        }
    }
}
