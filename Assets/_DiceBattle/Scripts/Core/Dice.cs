namespace DiceBattle.Core
{
    /// <summary>
    /// Represents a dice used in the game with specific functionality.
    /// </summary>
    public class Dice
    {
        private readonly System.Random _random;

        private DiceType _currentType;
        private bool _isLocked;
        
        public DiceType CurrentType => _currentType;
        
        /// <summary>
        /// Is it blocked for roll
        /// </summary>
        public bool IsLocked => _isLocked;

        public Dice()
        {
            _random = new System.Random();
            Roll();
        }

        /// <summary>
        /// Rolls the dice to randomly determine its type.
        /// </summary>
        public void Roll()
        {
            int randomValue = _random.Next(0, 4);
            _currentType = (DiceType)randomValue;
        }

        /// <summary>
        /// Unlocks the dice, allowing it to be rolled again or used in gameplay.
        /// </summary>
        public void Unlock()
        {
            _isLocked = false;
        }
    }
}