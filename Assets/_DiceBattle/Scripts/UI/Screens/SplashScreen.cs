using DiceBattle.UI;
using UnityEngine;

namespace DiceBattle
{
    public class SplashScreen : MonoBehaviour
    {
        [SerializeField] private Loader _loader;

        private void Start()
        {
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = 60;

            _loader.StartAnimation();
        }
    }
}
