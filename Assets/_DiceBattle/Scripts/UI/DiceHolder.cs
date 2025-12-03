using System.Collections.Generic;
using DiceBattle.Core;
using UnityEngine;

namespace DiceBattle
{
    public class DiceHolder : MonoBehaviour
    {
        [SerializeField] private List<Transform> _slots = new(5);

        private List<Dice> _occupied = new();

        public void PlaceSet(List<Dice> dices)
        {
            _occupied = new List<Dice>();

            for (int i = 0; i < _slots.Count; i++)
            {
                ToSlot(dices, i);
            }
        }

        // public void SetDiceToEmpty(Dice[] dices)
        // {
        //     foreach (var dice in dices)
        //     {
        //         for (int i = 0; i < _occupied.Count; i++)
        //         {
        //             if(_occupied[i] != null) continue;
        //
        //             _occupied[i] = dice;
        //         }
        //     }
        // }

        public List<Dice> GetSelectedDices()
        {
            var dices = new List<Dice>();

            for (int i = 0; i < _occupied.Count; i++)
            {
                if (_occupied[i] == null)
                {
                    continue;
                }

                if (_occupied[i].IsSelected == false)
                {
                    continue;
                }

                dices.Add(_occupied[i]);
                _occupied[i] = null;
            }

            return dices;
        }

        private void ToSlot(List<Dice> dices, int index)
        {
            dices[index].transform.SetParent(_slots[index]);
            dices[index].transform.localPosition = Vector3.zero;
            _occupied.Add(dices[index]);
        }
    }
}
