using DiceBattle.UI;

namespace DiceBattle.Core
{
    public class MatchData
    {
        public UnitData PlayerData;
        public UnitData EnemyData;

        public DiceList DiceList;

        public int RerollsPerTurn;
        public int RollsMade;

        public bool IsLastEnemy;
        public bool LastStandUsed;
    }
}
