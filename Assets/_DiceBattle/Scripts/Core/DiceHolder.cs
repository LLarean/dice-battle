using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DiceBattle.Core
{
    public class DiceHolder : MonoBehaviour
    {
        private readonly List<Dice> _occupied = new();
        private readonly List<GameObject> _slots = new();

        [SerializeField] private GameObject _diceSlot;
        [SerializeField] private Transform _slotSpawn;

        public event Action OnDiceToggled;

        public List<Dice> Occupied => _occupied;
        public List<Dice> Selected => _occupied.Where(dice => dice.IsSelected).ToList();

        public void Initialize(List<Dice> dice)
        {
            DestroySlots();
            InstantiateSlots(dice.Count);
            PlaceDiceToSlots(dice);
        }

        public void RepositionDice()
        {
            for (int i = 0; i < _occupied.Count; i++)
            {
                if (_occupied[i].gameObject.activeSelf == false)
                {
                    continue;
                }

                if (_occupied[i].transform.parent != _slots[i].transform)
                {
                    PlaceInSlot(_occupied[i], i);
                }
            }
        }

        public void SetSocketCount(int count)
        {
            for (int i = 0; i < _slots.Count; i++)
            {
                _slots[i].gameObject.SetActive(i < count);
            }
        }

        private void DestroySlots()
        {
            foreach (GameObject slot in _slots)
            {
                Destroy(slot);
            }

            _slots.Clear();
        }

        private void InstantiateSlots(int diceCount)
        {
            for (int i = 0; i < diceCount; i++)
            {
                GameObject diceSlot = Instantiate(_diceSlot, _slotSpawn);
                _slots.Add(diceSlot);
            }
        }

        private void PlaceDiceToSlots(List<Dice> dice)
        {
            ClearOccupiedDice();

            for (int i = 0; i < dice.Count; i++)
            {
                PlaceInSlot(dice[i], i);
                dice[i].OnToggled += HandleDiceToggle;
                _occupied.Add(dice[i]);
            }
        }

        private void ClearOccupiedDice()
        {
            foreach (Dice dice in _occupied)
            {
                dice.OnToggled -= HandleDiceToggle;
            }

            _occupied.Clear();
        }

        private void PlaceInSlot(Dice dice, int slotIndex)
        {
            dice.transform.SetParent(_slots[slotIndex].transform);
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
