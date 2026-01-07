using DiceBattle.Data;
using DiceBattle.Global;
using DiceBattle.UI;

namespace DiceBattle.Core
{
    public class Spawner
    {
        private readonly GameConfig _config;
        private readonly GameScreen _gameScreen;

        public Spawner(GameConfig config, GameScreen gameScreen)
        {
            _config = config;
            _gameScreen = gameScreen;
        }

        public UnitData SpawnEnemy()
        {
            int maxHealth = _config.Enemy.StartHealth + _config.Enemy.GrowthHealth * GameProgress.CompletedLevels;
            int damage = _config.Enemy.StartDamage + _config.Enemy.GrowthDamage * GameProgress.CompletedLevels;
            int armor = _config.Enemy.StartArmor + _config.Enemy.GrowthArmor * GameProgress.CompletedLevels;

            var enemyData = new UnitData
            {
                Title = $"Враг #{GameProgress.CompletedLevels + 1}", // TODO Translation
                Portrait = _config.Enemy.Portraits[GameProgress.CompletedLevels],
                MaxHealth = maxHealth,
                CurrentHealth = maxHealth,
                Damage = damage,
                Armor = armor,
            };

            enemyData.Log();
            _gameScreen.SetEnemyData(enemyData);
            return enemyData;
        }

        public UnitData SpawnHero()
        {
            var playerData = new UnitData
            {
                Title = "Герой (вы)", // TODO Translation
                Portrait = _config.Player.Portraits[0],
                MaxHealth = _config.Player.StartHealth,
                CurrentHealth = _config.Player.StartHealth,
                Damage = _config.Player.StartDamage,
                Armor = _config.Player.StartArmor,
            };

            playerData.Log();
            _gameScreen.SetPlayerData(playerData);
            return playerData;
        }
    }
}
