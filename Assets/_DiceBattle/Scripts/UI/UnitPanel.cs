using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DiceBattle
{
    public class UnitPanel : MonoBehaviour
    {
        [SerializeField] private TMP_Text _title;
        [SerializeField] private Image _portrait;
        [SerializeField] private UnitStats _unitStats;

        public void Construct(UnitData unitData)
        {
            _title.text = unitData.Title;
            _portrait.sprite = unitData.Portrait;
            
            _unitStats.HideAll();
            
            _unitStats.ShowHealth($"{unitData.HealthMin}/{unitData.HealthMax}");

            if (unitData.Attack != 0)
            {
                _unitStats.ShowAttack(unitData.Attack.ToString());
            }
            
            if (unitData.Defence != 0)
            {
                _unitStats.ShowDefence(unitData.Defence.ToString());
            }
        }
    }
}
