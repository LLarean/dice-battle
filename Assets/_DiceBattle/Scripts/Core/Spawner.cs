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
            UnitData enemyData = GetUnitData(_config.Enemy);
            enemyData.Title = $"Враг #{GameProgress.CompletedLevels + 1}"; // TODO Translation
            enemyData.Portrait = _config.Enemy.Portraits[GameProgress.CompletedLevels];

            enemyData.Log();
            _gameScreen.SetEnemyData(enemyData);
            return enemyData;
        }

        public UnitData SpawnHero()
        {
            UnitData playerData = GetUnitData(_config.Player);
            playerData.Title = "Герой (вы)"; // TODO Translation
            playerData.Portrait = _config.Player.Portraits[0];

            playerData.Log();
            _gameScreen.SetPlayerData(playerData);
            return playerData;
        }

        private UnitData GetUnitData(UnitConfig unitConfig)
        {
            int maxHealth = unitConfig.StartHealth + unitConfig.GrowthHealth * GameProgress.CompletedLevels;
            int damage = unitConfig.StartDamage + unitConfig.GrowthDamage * GameProgress.CompletedLevels;
            int armor = unitConfig.StartArmor + unitConfig.GrowthArmor * GameProgress.CompletedLevels;

            return new UnitData
            {
                MaxHealth = maxHealth,
                CurrentHealth = maxHealth,
                Damage = damage,
                Armor = armor,
            };
        }
    }
}
