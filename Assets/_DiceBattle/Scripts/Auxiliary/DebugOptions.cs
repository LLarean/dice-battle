#if UNITY_EDITOR
using DiceBattle.Global;
using UnityEngine;

namespace DiceBattle.Auxiliary
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
