using UnityEngine;

namespace DiceBattle.UI
{
    public class UnitStats : MonoBehaviour
    {
        [SerializeField] private StatItem _health;
        [SerializeField] private StatItem _attack;
        [SerializeField] private StatItem _armor;

        public StatItem Health => _health;
        public StatItem Attack => _attack;
        public StatItem Armor => _armor;

        public void SetHealth(string value, int bonus = 0, int loss = 0, bool isCriticalLoss = false) =>
            _health.SetValue(value, bonus, loss, isCriticalLoss);

        public void SetAttack(string value, int bonus = 0) => _attack.SetValue(value, bonus);

        public void SetArmor(string value, int bonus = 0) => _armor.SetValue(value, bonus);
    }
}
