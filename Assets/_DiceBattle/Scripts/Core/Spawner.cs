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
            UnitData source = _config.Enemies[GameData.CompletedLevels];
            UnitData enemyData = source.CloneAtFullHealth();

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

            DiceList diceList = GameData.GetEquippedAsDiceList();
            int armorBonus = diceList.DiceTypes.Count(r => r == DiceType.BaseArmor) * playerConfig.GrowthArmor;
            int damageBonus = diceList.DiceTypes.Count(r => r == DiceType.BaseDamage) * playerConfig.GrowthDamage;
            _gameScreen.SetPlayerEquipmentBonus(armorBonus, damageBonus);

            return playerData;
        }

        public UnitData RestoreEnemy(BattleSnapshot saved)
        {
            UnitData source = _config.Enemies[GameData.CompletedLevels];
            UnitData enemyData = source.CloneAtFullHealth();
            ApplySnapshot(enemyData, saved.EnemyState);

            enemyData.Log();
            _gameScreen.SetEnemyData(enemyData);
            return enemyData;
        }

        public UnitData RestoreHero(BattleSnapshot saved)
        {
            UnitConfig playerConfig = _config.GetPlayerConfig(GameData.SelectedCharacterClass);
            UnitData playerData = HeroFactory.Build(playerConfig);
            playerData.Name = "Герой (вы)"; // TODO Translation
            playerData.Portrait = playerConfig.Portraits[0];
            ApplySnapshot(playerData, saved.PlayerState);

            playerData.Log();
            _gameScreen.SetPlayerData(playerData);

            DiceList diceList = GameData.GetEquippedAsDiceList();
            int armorBonus = diceList.DiceTypes.Count(r => r == DiceType.BaseArmor) * playerConfig.GrowthArmor;
            int damageBonus = diceList.DiceTypes.Count(r => r == DiceType.BaseDamage) * playerConfig.GrowthDamage;
            _gameScreen.SetPlayerEquipmentBonus(armorBonus, damageBonus);

            return playerData;
        }

        private static void ApplySnapshot(UnitData unitData, UnitSnapshot snapshot)
        {
            unitData.CurrentHealth = snapshot.CurrentHealth;
            unitData.Damage = snapshot.Damage;
            unitData.Armor = snapshot.Armor;
        }
    }
}
