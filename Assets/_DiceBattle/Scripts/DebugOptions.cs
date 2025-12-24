#if UNITY_EDITOR
using UnityEngine;

namespace DiceBattle
{
    public class DebugOptions : MonoBehaviour
    {
        [SerializeField] private bool _needReset;

        private void Start()
        {
            if (_needReset)
            {
                GameProgress.ResetAll();
            }
        }
    }
}
#endif
