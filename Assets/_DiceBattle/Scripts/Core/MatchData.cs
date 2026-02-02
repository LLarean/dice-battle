using DiceBattle.UI;

namespace DiceBattle.Core
{
    public class MatchData
    {
        public UnitData PlayerData;
        public UnitData EnemyData;

        public RewardsData RewardsData;

        public int MaxDiceRerolls;
        public int RemainingDiceRerolls;

        public int PlayerHealthChange;
        public int EnemyHealthChange;
    }
}
