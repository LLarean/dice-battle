using System.Linq;
using DiceBattle.Data;
using DiceBattle.Global;
using DiceBattle.UI;
using UnityEngine;

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
            ApplyNewGamePlusMultiplier(enemyData);

            enemyData.Log();
            _gameScreen.SetEnemyData(enemyData);
            return enemyData;
        }

        public UnitData SpawnHero()
        {
            UnitConfig playerConfig = _config.GetPlayerConfig(GameData.SelectedCharacterClass);
            UnitData playerData = HeroFactory.Build(playerConfig);
            playerData.Name = playerConfig.Name;
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
            ApplyNewGamePlusMultiplier(enemyData);
            ApplySnapshot(enemyData, saved.EnemyState);

            enemyData.Log();
            _gameScreen.SetEnemyData(enemyData);
            return enemyData;
        }

        public UnitData RestoreHero(BattleSnapshot saved)
        {
            UnitConfig playerConfig = _config.GetPlayerConfig(GameData.SelectedCharacterClass);
            UnitData playerData = HeroFactory.Build(playerConfig);
            playerData.Name = playerConfig.Name;
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

        private void ApplyNewGamePlusMultiplier(UnitData enemyData)
        {
            float multiplier = _config.GetNewGamePlusMultiplier(GameData.NewGamePlusCycle);
            if (multiplier == 1f)
                return;

            enemyData.MaxHealth = Mathf.RoundToInt(enemyData.MaxHealth * multiplier);
            enemyData.CurrentHealth = enemyData.MaxHealth;
            enemyData.Damage = Mathf.RoundToInt(enemyData.Damage * multiplier);
            enemyData.Armor = Mathf.RoundToInt(enemyData.Armor * multiplier);
        }
    }
}
