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

        public void Log()
        {
            Debug.Log("---UnitData---");
            Debug.Log("Title = " + Title );
            Debug.Log("Portrait.name = " + Portrait.name);
            Debug.Log("MaxHealth = " + MaxHealth);
            Debug.Log("CurrentHealth = " + CurrentHealth);
            Debug.Log("Attack = " + Attack);
            Debug.Log("Defense = " + Defense);
            Debug.Log("---End---");
        }
    }
}
