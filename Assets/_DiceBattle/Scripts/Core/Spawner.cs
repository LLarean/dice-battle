using System.Linq;
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
            UnitData source = _config.Enemies[GameProgress.CompletedLevels];

            var enemyData = new UnitData
            {
                Title = source.Title,
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
            UnitData playerData = GetHeroUnitData(_config.Player);
            playerData.Title = "Герой (вы)"; // TODO Translation
            playerData.Portrait = _config.Player.Portraits[0];

            playerData.Log();
            _gameScreen.SetPlayerData(playerData);
            return playerData;
        }

        private UnitData GetHeroUnitData(UnitConfig unitConfig)
        {
            RewardsData rewardsData = GameProgress.GetReceivedRewards();

            int doubleHealthCount = rewardsData.RewardTypes.Count(r => r == RewardType.DoubleHealth);
            int additionalHealth = unitConfig.StartHealth * doubleHealthCount;
            int maxHealth = unitConfig.StartHealth + additionalHealth;

            int baseDamageCount = rewardsData.RewardTypes.Count(r => r == RewardType.BaseDamage);
            int damage = unitConfig.StartDamage + baseDamageCount;

            int baseArmorCount = rewardsData.RewardTypes.Count(r => r == RewardType.BaseArmor);
            int armor = unitConfig.StartArmor + baseArmorCount;

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
