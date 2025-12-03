using System.Collections.Generic;
using DiceBattle.Core;
using UnityEngine;

namespace DiceBattle
{
    public class DiceTray : MonoBehaviour
    {
        [SerializeField] private List<Transform> _slots = new(5);

        private List<Dice> _occupied = new(5);
        
        public void SetAllDices(Dice[] dices)
        {
            for (int i = 0; i < _slots.Count; i++)
            {
                ToSlot(dices, i);
            }
        }

        public void SetDiceToEmpty(Dice[] dices)
        {
            foreach (var dice in dices)
            {
                for (int i = 0; i < _occupied.Count; i++)
                {
                    if(_occupied[i] != null) continue;

                    _occupied[i] = dice;
                }
            }
        }
        
        public List<Dice> GetUnlockedDices()
        {
            List<Dice> dices = new List<Dice>();

            for (int i = 0; i < _occupied.Count; i++)
            {
                if (_occupied[i] == null) continue;
                if (_occupied[i].IsLocked) continue;
                
                dices.Add(_occupied[i]);
                _occupied[i] = null;
            }

            return dices;
        }

        private void ToSlot(Dice[] dices, int i)
        {
            dices[i].transform.SetParent(_slots[i]);
            dices[i].transform.localPosition = Vector3.zero;
            _occupied[i] = dices[i];
        }
    }
}