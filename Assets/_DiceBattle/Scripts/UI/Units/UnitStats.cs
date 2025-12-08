using TMPro;
using UnityEngine;

namespace DiceBattle
{
    public class UnitStats : MonoBehaviour
    {
        [SerializeField] private GameObject _health;
        [SerializeField] private GameObject _attack;
        [SerializeField] private GameObject _defence;
        [Space]
        [SerializeField] private TMP_Text _healthValue;
        [SerializeField] private TMP_Text _attackValue;
        [SerializeField] private TMP_Text _defenceValue;

        public void ShowHealth(string value)
        {
            _healthValue.text = value;
            _health.gameObject.SetActive(true);
            ResetGameObject();
        }

        public void ShowAttack(string value)
        {
            _attackValue.text = value;
            _attack.gameObject.SetActive(true);
            ResetGameObject();
        }

        public void ShowDefense(string value)
        {
            _defenceValue.text = value;
            _defence.gameObject.SetActive(true);
            ResetGameObject();
        }

        public void HideHealth() => _health.SetActive(false);

        public void HideAttack() => _attack.SetActive(false);

        public void HideDefense() => _defence.SetActive(false);

        private void ResetGameObject()
        {
            gameObject.SetActive(false);
            gameObject.SetActive(true);
        }
    }
}
