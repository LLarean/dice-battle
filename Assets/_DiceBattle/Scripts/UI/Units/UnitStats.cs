using UnityEngine;

namespace DiceBattle.UI
{
    public class UnitStats : MonoBehaviour
    {
        [SerializeField] private StatItem _health;
        [SerializeField] private StatItem _attack;
        [SerializeField] private StatItem _armor;

        public void SetHealth(string value, int bonus = 0) => _health.SetValue(value, bonus);

        public void SetAttack(string value, int bonus = 0) => _attack.SetValue(value, bonus);

        public void SetArmor(string value, int bonus = 0) => _armor.SetValue(value, bonus);
    }
}
