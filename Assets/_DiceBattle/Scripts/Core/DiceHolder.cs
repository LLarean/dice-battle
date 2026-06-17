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
        private readonly Dictionary<Dice, Action> _slotHandlers = new();

        [SerializeField] private GameObject _diceSlot;
        [SerializeField] private Transform _slotSpawn;

        public event Action OnDiceToggled;
        public event Action<Dice> OnSlotDiceClicked;

        public List<Dice> Occupied => _occupied;
        public List<Dice> Selected => _occupied.Where(dice => dice.IsSelected).ToList();

        public int FreeSlotCount => _slots.Count(IsSlotFree);

        private static bool IsSlotFree(GameObject slot) =>
            slot.GetComponentInChildren<Dice>() == null;

        public void Initialize(int diceCount)
        {
            DestroySlots();
            InstantiateSlots(diceCount);
        }

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

        public void AddSocket()
        {
            for (int i = _slots.Count - 1; i < _slots.Count + 1; i++)
            {
                GameObject diceSlot = Instantiate(_diceSlot, _slotSpawn);
                _slots.Add(diceSlot);
            }
        }

        public void RemoveSocket()
        {
            _slots.RemoveAt(_slots.Count - 1);
        }

        private void DestroySlots()
        {
            foreach (GameObject slot in _slots)
            {
                Destroy(slot);
            }

            _slots.Clear();
            _slotHandlers.Clear();
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

        public void PlaceInSlot(Dice dice, int slotIndex)
        {
            dice.transform.SetParent(_slots[slotIndex].transform);
            dice.transform.localPosition = Vector3.zero;
        }

        public bool TryEquip(Dice dice)
        {
            int freeSlot = _slots.FindIndex(IsSlotFree);
            if (freeSlot < 0)
            {
                return false;
            }

            PlaceInSlot(dice, freeSlot);

            Action handler = () => OnSlotDiceClicked?.Invoke(dice);
            _slotHandlers[dice] = handler;
            dice.OnToggled += handler;
            return true;
        }

        public Dice TryEquipCopy(Dice source)
        {
            int freeSlot = _slots.FindIndex(IsSlotFree);
            if (freeSlot < 0)
            {
                return null;
            }

            Dice copy = Instantiate(source, _slots[freeSlot].transform);
            copy.transform.localPosition = Vector3.zero;

            Action handler = () => OnSlotDiceClicked?.Invoke(copy);
            _slotHandlers[copy] = handler;
            copy.OnToggled += handler;
            return copy;
        }

        public void Unequip(Dice dice)
        {
            if (_slotHandlers.TryGetValue(dice, out Action handler))
            {
                dice.OnToggled -= handler;
                _slotHandlers.Remove(dice);
            }
        }

        public void RemoveCopy(Dice copy)
        {
            Unequip(copy);
            Destroy(copy.gameObject);
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
