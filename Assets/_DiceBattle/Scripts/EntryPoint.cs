using DiceBattle.Data;
using UnityEngine;

namespace DiceBattle
{
    public class EntryPoint : MonoBehaviour
    {
        [SerializeField] private GameConfig _config;
        [Space]
        [SerializeField] private UnitView _player;
        [SerializeField] private UnitView _enemy;
        [Space]
        [SerializeField] private Sprite _knight;

        private void Start()
        {
            UnitModel unitModel = new UnitModel
            {
                Title = "Player(You)",
                Portrait = _knight,
                MaxHealth = _config.PlayerStartHP,
                CurrentHealth = _config.PlayerStartHP,
                Attack = 0,
                Defence = 0
            };

            UnitPresenter unitPresenter = new UnitPresenter(unitModel, _player);
            unitPresenter.Subscribe();
            unitPresenter.Show();
            
            
        }
    }

    public class GamePresenter
    {
        public GamePresenter(UnitView player, Unit)
        {
            
        }
    }
}
