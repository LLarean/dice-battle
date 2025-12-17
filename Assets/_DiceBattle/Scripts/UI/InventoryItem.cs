using UnityEngine;

namespace DiceBattle
{
    public class InventoryItem : MonoBehaviour
    {
        [SerializeField] private RewardType _rewardType;

        public RewardType RewardType => _rewardType;
    }
}
