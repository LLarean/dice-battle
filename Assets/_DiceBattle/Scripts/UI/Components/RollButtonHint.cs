using DiceBattle.Global;
using UnityEngine;

namespace DiceBattle.UI
{
    public class RollButtonHint : MonoBehaviour
    {
        [SerializeField] private ButtonShine _shine;
        [Space]
        [SerializeField] private float _idleThreshold = 10f;

        private float _idleTimer;
        private bool _isShining;
        private bool _isPaused;

        public void Notify()
        {
            _idleTimer = 0f;
            StopShine();
        }

        public void SetPaused(bool isPaused)
        {
            _isPaused = isPaused;

            if (isPaused)
            {
                StopShine();
            }
            else
            {
                _idleTimer = 0f;
            }
        }

        private void Update()
        {
            if (_isPaused || _isShining)
            {
                return;
            }

            _idleTimer += Time.unscaledDeltaTime;

            if (_idleTimer >= _idleThreshold)
            {
                StartShine();
            }
        }

        private void OnEnable()
        {
            _idleTimer = GameData.HasEverRolledDice ? 0f : _idleThreshold;
        }

        private void OnDisable() => StopShine();

        private void StartShine()
        {
            _isShining = true;
            _shine.enabled = true;
        }

        private void StopShine()
        {
            if (_isShining == false)
            {
                return;
            }

            _isShining = false;
            _idleTimer = 0f;
            _shine.enabled = false;
        }
    }
}
