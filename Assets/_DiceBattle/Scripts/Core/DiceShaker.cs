using System;
using System.Collections.Generic;
using UnityEngine;

namespace DiceBattle.Core
{
    public class DiceShaker : MonoBehaviour
    {
        public event Action OnRollCompleted;

        public void Roll(List<Dice> allDices)
        {
            throw new NotImplementedException();
        }
    }
}
