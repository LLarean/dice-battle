using DiceBattle.UI;
using UnityEngine;
using UnityEngine.UI;

namespace DiceBattle
{
    public class TournamentLevel : MonoBehaviour
    {
        private const string CurrentLabel = "vs";

        [SerializeField] private UnitPanel _player;
        [SerializeField] private UnitPanel _enemy;
        [SerializeField] private Image _blackout;
        [SerializeField] private LevelCounter _levelCounter;

        public void SetUnits(UnitData player, UnitData enemy)
        {
            _player.SetUnitData(player);
            _enemy.SetUnitData(enemy);
        }

        public void SetCurrentState()
        {
            _blackout.gameObject.SetActive(false);
            _levelCounter.SetEmptyState(CurrentLabel);
        }

        public void SetLockedState(int number)
        {
            _blackout.gameObject.SetActive(true);
            _levelCounter.SetEmptyState(number.ToString());
        }

        public void SetPassedState()
        {
            _blackout.gameObject.SetActive(true);
            _levelCounter.SetSuccessState();
        }

        public void SetDefeatedState()
        {
            _blackout.gameObject.SetActive(false);
            _levelCounter.SetDefeatState();
        }
    }
}
