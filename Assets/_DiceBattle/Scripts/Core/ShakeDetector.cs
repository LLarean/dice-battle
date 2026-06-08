using System;
using UnityEngine;

namespace DiceBattle.Core
{
    public class ShakeDetector : MonoBehaviour
    {
        [SerializeField] private float _threshold = 2.5f;
        [SerializeField] private float _cooldown = 1.5f;

        public event Action OnShake;

        private float _cooldownTimer;

#if !UNITY_ANDROID && !UNITY_IOS
        private void Awake() => enabled = false;
#else
        private void Update()
        {
            if (_cooldownTimer > 0f)
            {
                _cooldownTimer -= Time.deltaTime;
                return;
            }

            if (Input.acceleration.sqrMagnitude > _threshold * _threshold)
            {
                _cooldownTimer = _cooldown;
                OnShake?.Invoke();
            }
        }
#endif
    }
}
