using UnityEngine;

namespace DiceBattle.UI
{
    public class UnitStats : MonoBehaviour
    {
        [SerializeField] private StatItem _health;
        [SerializeField] private StatItem _attack;
        [SerializeField] private StatItem _armor;

        public void ShowHealth(string value)
        {
            _health.SetValue(value);
            _health.gameObject.SetActive(true);
            ResetGameObject();
        }

        public void ShowAttack(string value)
        {
            _attack.SetValue(value);
            _attack.gameObject.SetActive(true);
            ResetGameObject();
        }

        public void ShowArmor(string value)
        {
            _armor.SetValue(value);
            _armor.gameObject.SetActive(true);
            ResetGameObject();
        }

        public void HideHealth() => _health.gameObject.SetActive(false);

        public void HideAttack() => _attack.gameObject.SetActive(false);

        public void HideArmor() => _armor.gameObject.SetActive(false);

        private void ResetGameObject()
        {
            gameObject.SetActive(false);
            gameObject.SetActive(true);
        }
    }
}
