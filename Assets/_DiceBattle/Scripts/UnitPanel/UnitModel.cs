using UnityEngine;

namespace DiceBattle
{
    public class UnitModel
    {
        public string Title;
        public Sprite Portrait;
        
        public int MaxHealth;
        public Observable<int> CurrentHealth;
        
        public int Attack;
        public int Defence;
    }
}