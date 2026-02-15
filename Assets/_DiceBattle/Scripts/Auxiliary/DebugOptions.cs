#if UNITY_EDITOR
using DiceBattle.Global;
using DiceBattle.UI;
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
            GameProgress.SaveReceivedReward(_diceType);
        }

        [Button]
        private void AddEquippedReward()
        {
            DiceList equippedRewards = GameProgress.LoadEquippedRewards();
            equippedRewards.DiceTypes.Add(_diceType);
            GameProgress.SaveEquippedRewards(equippedRewards);
        }

        [Button]
        private void AddAllRewards()
        {
            DiceList randomRewards = GameProgress.LoadRandomRewards();

            foreach (DiceType randomReward in randomRewards.DiceTypes)
            {
                GameProgress.SaveReceivedReward(randomReward);
            }
        }

        [Button]
        private void EquipAllRewards()
        {
            DiceList randomRewards = GameProgress.LoadRandomRewards();
            GameProgress.SaveEquippedRewards(randomRewards);
        }
    }
}
#endif
