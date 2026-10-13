#if UNITY_STANDALONE && !UNITY_EDITOR
using UnityEngine;

namespace DiceBattle.Auxiliary
{
    public static class StandaloneWindow
    {
        private const float _displayHeightShare = 0.85f;
        private const float _aspect = 9f / 16f;

        // The layout is portrait only, so on a desktop the game runs in a phone-shaped window.
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void FitToDisplay()
        {
            int height = Mathf.RoundToInt(Display.main.systemHeight * _displayHeightShare);
            int width = Mathf.RoundToInt(height * _aspect);
            UnityEngine.Screen.SetResolution(width, height, FullScreenMode.Windowed);
        }
    }
}
#endif
