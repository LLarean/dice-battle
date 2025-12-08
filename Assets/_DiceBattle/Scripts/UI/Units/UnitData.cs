using UnityEngine;

namespace DiceBattle
{
    public class UnitData
    {
        public string Title;
        public Sprite Portrait;

        public int MaxHealth;
        public int CurrentHealth;

        public int Attack;
        public int Defense;

        public void LogUnitData()
        {
            Debug.Log("unitData.Title = " + Title);
            Debug.Log("unitData.Portrait.name = " + Portrait.name);
            Debug.Log("unitData.MaxHealth = " + MaxHealth);
            Debug.Log("unitData.CurrentHealth = " + CurrentHealth);
            Debug.Log("unitData.Attack = " + Attack);
            Debug.Log("unitData.Defense = " + Defense);
        }
    }
}
