using DiceBattle.UI;
using UnityEngine;

namespace DiceBattle
{
    public class SplashScreen : MonoBehaviour
    {
        [SerializeField] private Loader _loader;
        [SerializeField] private SplashTip _splashTip;

        private void Awake()
        {
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = 60;
        }

        private void Start()
        {
            _loader.StartAnimation();
            _splashTip.ShowRandomTip();
        }
    }
}
