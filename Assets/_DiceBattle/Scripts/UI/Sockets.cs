using DiceBattle.Core;
using UnityEngine;

namespace DiceBattle
{
    public class Sockets : MonoBehaviour
    {
        [SerializeField] private Transform[] _slots = new Transform[5];

        private Dice[] _installed;
        
        public void SetAll(Dice[] dices)
        {
            _installed = new Dice[dices.Length];

            for (int i = 0; i < _slots.Length; i++)
            {
                dices[i].transform.SetParent(_slots[i]);
                dices[i].transform.localPosition = Vector3.zero;
                _installed[i] = dices[i];
            }
        }
    }
}