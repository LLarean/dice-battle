#if UNITY_EDITOR
using DiceBattle.Global;
using UnityEngine;

namespace DiceBattle.Auxiliary
{
    public class DebugOptions : MonoBehaviour
    {
        [SerializeField] private bool _needResetAll;
        [SerializeField] private RewardType _rewardType;

        private void Start()
        {
            if (_needResetAll)
            {
                GameProgress.ResetAll();
                GameSettings.ResetVolume();
            }
        }

        [ContextMenu("AddReward")]
        private void AddReward()
        {
            GameProgress.AddReceivedReward(_rewardType);
        }
    }
}
#endif
