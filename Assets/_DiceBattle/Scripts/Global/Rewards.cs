using System.Collections.Generic;
using DiceBattle.UI;
using UnityEngine;

namespace DiceBattle.Global
{
    public record ReceivedRewards
    {
        public void AddReceived(RewardType rewardType)
        {
            string rewardsJson = PlayerPrefs.GetString(PlayerPrefsKeys.ReceivedRewards, null);

            RewardsData rewardsData = string.IsNullOrEmpty(rewardsJson) == false
                ? JsonUtility.FromJson<RewardsData>(rewardsJson)
                : new RewardsData();

            rewardsData.RewardTypes ??= new List<RewardType>();
            rewardsData.RewardTypes.Add(rewardType);

            PlayerPrefs.SetString(PlayerPrefsKeys.ReceivedRewards, JsonUtility.ToJson(rewardsData));

            rewardsJson = PlayerPrefs.GetString(PlayerPrefsKeys.ReceivedRewards, "{}");
            Debug.Log("<color=yellow>ReceivedRewards:</color>" + rewardsJson);
        }

        public void AddRewardList(RewardsData rewardsData)
        {
            PlayerPrefs.SetString(PlayerPrefsKeys.RewardsList, JsonUtility.ToJson(rewardsData));
        }

        public RewardsData GetRewardList()
        {
            string rewardsJson = PlayerPrefs.GetString(PlayerPrefsKeys.RewardsList, "{}");
            RewardsData rewardsData = JsonUtility.FromJson<RewardsData>(rewardsJson) ?? new RewardsData();

            rewardsData.RewardTypes ??= new List<RewardType>();
            return rewardsData;
        }

        public void ResetRewardsList() => PlayerPrefs.DeleteKey(PlayerPrefsKeys.RewardsList);

        public static RewardsData GetReceivedRewards()
        {
            string rewardsJson = PlayerPrefs.GetString(PlayerPrefsKeys.ReceivedRewards, "{}");
            RewardsData rewardsData = JsonUtility.FromJson<RewardsData>(rewardsJson) ?? new RewardsData();

            rewardsData.RewardTypes ??= new List<RewardType>();
            return rewardsData;
        }

        public static void ResetReceivedRewards() => PlayerPrefs.DeleteKey(PlayerPrefsKeys.ReceivedRewards);
    }
}
