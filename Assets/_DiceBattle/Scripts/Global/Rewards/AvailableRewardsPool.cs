using System;
using System.Collections.Generic;
using System.Linq;
using DiceBattle.UI;
using UnityEngine;
using Random = System.Random;

namespace DiceBattle.Global
{
    public static class AvailableRewardsPool
    {
        private const string _playerPrefsKey = PlayerPrefsKeys.RewardsList;

        private static readonly Random _random = new();

        public static DiceList Load()
        {
            string json = PlayerPrefs.GetString(_playerPrefsKey, null);

            if (string.IsNullOrEmpty(json))
            {
                return new DiceList();
            }

            DiceList data = JsonUtility.FromJson<DiceList>(json);

            if (data == null)
            {
                Debug.LogWarning("Failed to deserialize rewards data, creating new");
                return new DiceList();
            }

            data.DiceTypes ??= new List<DiceType>();
            data.DiceTypes.RemoveAll(type => Enum.IsDefined(typeof(DiceType), type) == false);
            return data;
        }

        public static void Save(DiceList diceList)
        {
            if (diceList == null)
            {
                Debug.LogWarning("Attempted to save null rewards data");
                return;
            }

            diceList.DiceTypes ??= new List<DiceType>();
            string json = JsonUtility.ToJson(diceList);
            PlayerPrefs.SetString(_playerPrefsKey, json);
            PlayerPrefs.Save();
        }

        public static void Log()
        {
#if UNITY_EDITOR
            string randomRewardsJson = PlayerPrefs.GetString(_playerPrefsKey, "{}");
            Debug.Log("<color=yellow>AvailableRewardsPool: </color>" + randomRewardsJson);
#endif
        }

        public static void Clear() => PlayerPrefs.DeleteKey(_playerPrefsKey);

        // The pool is a flat list of offers, each of `count` different types; a level reads its offer by index.
        public static List<DiceType> GetRewardsRange(DiceList diceList, int startIndex, int count)
        {
            int incompleteOffer = diceList.DiceTypes.Count % count;
            diceList.DiceTypes.RemoveRange(diceList.DiceTypes.Count - incompleteOffer, incompleteOffer);

            while (diceList.DiceTypes.Count < startIndex + count)
            {
                diceList.DiceTypes.AddRange(GetRandomOffer(count));
            }

            List<DiceType> result = diceList.DiceTypes.GetRange(startIndex, count);
            Save(diceList);
            return result;
        }

        private static List<DiceType> GetRandomOffer(int count)
        {
            List<DiceType> candidates = Enum.GetValues(typeof(DiceType))
                .Cast<DiceType>()
                .Where(type => type != DiceType.Default)
                .ToList();

            var offer = new List<DiceType>();

            while (offer.Count < count && candidates.Count > 0)
            {
                int roll = _random.Next(candidates.Sum(type => type.GetRarity().GetDropWeight()));
                int index = 0;

                while (roll >= candidates[index].GetRarity().GetDropWeight())
                {
                    roll -= candidates[index].GetRarity().GetDropWeight();
                    index++;
                }

                offer.Add(candidates[index]);
                candidates.RemoveAt(index);
            }

            return offer;
        }
    }
}
