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
            UnitData playerData = GetHeroUnitData(_config.Player);
            playerData.Name = "Герой (вы)"; // TODO Translation
            playerData.Portrait = _config.Player.Portraits[0];

            playerData.Log();
            _gameScreen.SetPlayerData(playerData);
            return playerData;
        }

        // TODO Refactor this
        private UnitData GetHeroUnitData(UnitConfig unitConfig)
        {
            DiceList diceList = GameData.GetInventory();

            int doubleHealthCount = diceList.DiceTypes.Count(r => r == DiceType.BaseHealth);
            // int additionalHealth = unitConfig.StartHealth * doubleHealthCount;
            int maxHealth = unitConfig.StartHealth * doubleHealthCount;
            // int maxHealth = unitConfig.StartHealth + additionalHealth;

            // Debug.Log("diceList.DiceTypes.Count(r => r == DiceType.BaseHealth) = " + diceList.DiceTypes.Count(r => r == DiceType.BaseHealth));
            // Debug.Log("additionalHealth = " + additionalHealth);
            // Debug.Log("maxHealth = " + maxHealth);
            // Debug.Log("unitConfig.StartHealth = " + unitConfig.StartHealth);



            int baseDamageCount = diceList.DiceTypes.Count(r => r == DiceType.BaseDamage);
            int damage = baseDamageCount > 1 ? unitConfig.StartDamage + baseDamageCount -1 : unitConfig.StartDamage;
            // int damage = unitConfig.StartDamage + baseDamageCount;

            int baseArmorCount = diceList.DiceTypes.Count(r => r == DiceType.BaseArmor);
            int armor = baseArmorCount > 1 ? unitConfig.StartArmor + baseArmorCount -1 : unitConfig.StartArmor;
            // int armor = unitConfig.StartArmor + baseArmorCount;

            // Debug.Log("diceList.DiceTypes.Count(r => r == DiceType.BaseDamage) = " + diceList.DiceTypes.Count(r => r == DiceType.BaseDamage));
            // Debug.Log("diceList.DiceTypes.Count(r => r == DiceType.BaseArmor) = " + diceList.DiceTypes.Count(r => r == DiceType.BaseArmor));

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
