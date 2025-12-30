using UnityEngine;

namespace DiceBattle.UI
{
    public class InventoryItem : MonoBehaviour
    {
        [SerializeField] private RewardType _rewardType;

        public RewardType RewardType => _rewardType;
    }
}
