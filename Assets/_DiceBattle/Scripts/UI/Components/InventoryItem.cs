using TMPro;
using UnityEngine;

namespace DiceBattle.UI
{
    public class InventoryItem : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _title;
        private RewardType _rewardType;

        public RewardType RewardType => _rewardType;

        public void Construct(RewardType rewardType)
        {
            _rewardType = rewardType;
            // TODO Add title display and translation into different languages
            _title.text = rewardType.ToString();
        }
    }
}
