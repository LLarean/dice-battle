using System.Collections.Generic;
using UnityEngine;

namespace DiceBattle.Data
{
    [CreateAssetMenu(fileName = "SplashTipsConfig", menuName = "Dice Battle/Splash Tips Config", order = 1)]
    public class SplashTipsConfig : ScriptableObject
    {
        [SerializeField] private List<string> _tips = new();

        public List<string> Tips => _tips;
    }
}
