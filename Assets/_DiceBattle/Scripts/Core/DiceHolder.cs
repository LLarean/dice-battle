using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DiceBattle.Core
{
    public class DiceHolder : MonoBehaviour
    {
        private readonly List<Dice> _occupied = new();

        [SerializeField] private List<Transform> _slots = new(5);

        public event Action OnDiceToggled;

        public List<Dice> Occupied => _occupied;
        public List<Dice> Selected => _occupied.Where(dice => dice.IsSelected).ToList();

        public void Initialize(List<Dice> dice)
        {
            _occupied.Clear();

            for (int i = 0; i < dice.Count; i++)
            {
                PlaceInSlot(dice[i], i);
                dice[i].OnToggled += HandleDiceToggle;
                _occupied.Add(dice[i]);
            }
        }

        public void RepositionDice()
        {
            for (int i = 0; i < _occupied.Count; i++)
            {
                if (_occupied[i].transform.parent != _slots[i].transform)
                {
                    _occupied[i].Roll();
                    PlaceInSlot(_occupied[i], i);
                }
            }
        }

        private void PlaceInSlot(Dice dice, int slotIndex)
        {
            dice.transform.SetParent(_slots[slotIndex]);
            dice.transform.localPosition = Vector3.zero;
        }

        private void HandleDiceToggle() => OnDiceToggled?.Invoke();

        private void OnDestroy()
        {
            foreach (Dice dice in _occupied)
            {
                dice.OnToggled -= HandleDiceToggle;
            }
        }
    }
}
