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
            UnitData source = _config.Enemies[GameData.CompletedLevels];

            var enemyData = new UnitData
            {
                Name = source.Name,
                Portrait = source.Portrait,
                Background = source.Background,
                MaxHealth = source.MaxHealth,
                CurrentHealth = source.MaxHealth,
                Damage = source.Damage,
                Armor = source.Armor,

            };

            enemyData.Log();
            _gameScreen.SetEnemyData(enemyData);
            return enemyData;
        }

        public UnitData SpawnHero()
        {
            UnitConfig playerConfig = _config.GetPlayerConfig(GameData.SelectedCharacterClass);
            UnitData playerData = HeroFactory.Build(playerConfig);
            playerData.Name = "Герой (вы)"; // TODO Translation
            playerData.Portrait = playerConfig.Portraits[0];

            playerData.Log();
            _gameScreen.SetPlayerData(playerData);
            return playerData;
        }
    }
}
