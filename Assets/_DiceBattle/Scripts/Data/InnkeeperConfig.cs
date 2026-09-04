using Assets.SimpleLocalization.Scripts;
using UnityEngine;

namespace DiceBattle.Data
{
    [CreateAssetMenu(fileName = "InnkeeperConfig", menuName = "Dice Battle/Innkeeper Config", order = 1)]
    public class InnkeeperConfig : ScriptableObject
    {
        [SerializeField] private string _keyPrefix = "innkeeper.msg";

        public int Count => LocalizationManager.CountIndexedKeys(_keyPrefix);

        public string GetKey(int index) => $"{_keyPrefix}[{index}]";
    }
}
