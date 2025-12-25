#if UNITY_EDITOR
using UnityEngine;

namespace DiceBattle
{
    public class DebugOptions : MonoBehaviour
    {
        [SerializeField] private bool _needResetAll;

        private void Start()
        {
            if (_needResetAll)
            {
                GameProgress.ResetAll();
                GameSettings.ResetVolume();
            }
        }
    }
}
#endif
