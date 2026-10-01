using DiceBattle.Core;

namespace DiceBattle.Auxiliary
{
    /// <summary>
    /// Cheats read by gameplay code. Only the editor-only DebugOptions writes them, so builds always see the defaults.
    /// </summary>
    public static class DebugOverrides
    {
        public const int InfiniteRerolls = 999;

        public static bool IsInstaWin;
        public static bool IsInstaLose;
        public static bool HasInfiniteRerolls;
        public static DiceValue[] ForcedFaces;

        public static bool TryGetForcedFace(int boardIndex, out DiceValue face)
        {
            bool isForced = ForcedFaces != null && boardIndex >= 0 && boardIndex < ForcedFaces.Length;
            face = isForced ? ForcedFaces[boardIndex] : default;
            return isForced;
        }
    }
}
