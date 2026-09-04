using Assets.SimpleLocalization.Scripts;
using UnityEngine;

namespace DiceBattle.Data
{
    [CreateAssetMenu(fileName = "SplashTipsConfig", menuName = "Dice Battle/Splash Tips Config", order = 1)]
    public class SplashTipsConfig : ScriptableObject
    {
        [SerializeField] private string _keyPrefix = "splash_tips.msg";

        public int Count => LocalizationManager.CountIndexedKeys(_keyPrefix);

        public string GetKey(int index) => $"{_keyPrefix}[{index}]";
    }
}
