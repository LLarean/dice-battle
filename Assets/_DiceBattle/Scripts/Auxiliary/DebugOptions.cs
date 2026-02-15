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
                GameData.ResetAll();
                GameSettings.ResetVolume();
            }
        }

        [Button]
        private void AddReward()
        {
            GameData.SaveReceivedReward(_diceType);
        }

        [Button]
        private void AddEquippedReward()
        {
            DiceList equippedRewards = GameData.LoadEquippedRewards();
            equippedRewards.DiceTypes.Add(_diceType);
            GameData.SaveEquippedRewards(equippedRewards);
        }

        [Button]
        private void AddAllRewards()
        {
            DiceList randomRewards = GameData.LoadRandomRewards();

            foreach (DiceType randomReward in randomRewards.DiceTypes)
            {
                GameData.SaveReceivedReward(randomReward);
            }
        }

        [Button]
        private void EquipAllRewards()
        {
            DiceList randomRewards = GameData.LoadRandomRewards();
            GameData.SaveEquippedRewards(randomRewards);
        }
    }
}
#endif
