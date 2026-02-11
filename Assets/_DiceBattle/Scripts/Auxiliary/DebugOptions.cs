#if UNITY_EDITOR
using DiceBattle.Global;
using NaughtyAttributes;
using UnityEngine;

namespace DiceBattle.Auxiliary
{
    public class DebugOptions : MonoBehaviour
    {
        [SerializeField] private bool _needResetAll;
        [SerializeField] private DiceType _diceType;

        private void Start()
        {
            if (_needResetAll)
            {
                GameProgress.ResetAll();
                GameSettings.ResetVolume();
            }
        }

        [Button]
        private void AddReward()
        {
            GameProgress.AddReceivedReward(_diceType);
        }
    }
}
#endif
