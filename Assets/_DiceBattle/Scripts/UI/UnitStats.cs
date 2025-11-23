using TMPro;
using UnityEngine;

namespace DiceBattle
{
    public class UnitStats : MonoBehaviour
    {
        [SerializeField] private GameObject _hp;
        [SerializeField] private GameObject _attack;
        [SerializeField] private GameObject _defence;
        [Space]
        [SerializeField] private TMP_Text _hpValue;
        [SerializeField] private TMP_Text _attackValue;
        [SerializeField] private TMP_Text _defenceValue;

        public void HideAll()
        {
            _hp.SetActive(false);
            _attack.SetActive(false);
            _defence.SetActive(false);
        }

        public void ShowHP(string value)
        {
            _hpValue.text = value;
            _hp.gameObject.SetActive(true);
        }
        
        public void ShowAttack(string value)
        {
            _attackValue.text = value;
            _attack.gameObject.SetActive(true);
        }
        
        public void ShowDefence(string value)
        {
            _defenceValue.text = value;
            _defence.gameObject.SetActive(true);
        }
    }
}