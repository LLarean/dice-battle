using DiceBattle.UI;
using UnityEngine;
using UnityEngine.UI;

namespace DiceBattle
{
    public class TournamentLevel : MonoBehaviour
    {
        [SerializeField] private UnitPanel _player;
        [SerializeField] private UnitPanel _enemy;
        [SerializeField] private Image _blackout;
        [SerializeField] private LevelCounter _levelCounter;
    }
}
