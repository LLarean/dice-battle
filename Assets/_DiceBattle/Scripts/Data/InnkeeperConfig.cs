using System.Collections.Generic;
using UnityEngine;

namespace DiceBattle.Data
{
    [CreateAssetMenu(fileName = "InnkeeperConfig", menuName = "Dice Battle/Innkeeper Config", order = 1)]
    public class InnkeeperConfig : ScriptableObject
    {
        [SerializeField] private List<string> _messages = new();

        public List<string> Messages => _messages;
    }
}
