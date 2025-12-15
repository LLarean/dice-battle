using DiceBattle.Audio;
using GameSignals;
using UnityEngine;
using UnityEngine.UI;

namespace DiceBattle
{
    public class OptionsWindow : MonoBehaviour
    {
        [SerializeField] private Transform _substrate;
        [Space]
        [SerializeField] private Button _close;

        private void Start()
        {
            _close.onClick.AddListener(HandleCloseClick);
            _substrate.gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            _close.onClick.RemoveAllListeners();
        }

        private void HandleCloseClick()
        {
            SignalSystem.Raise<ISoundHandler>(handler => handler.PlaySound(SoundType.Click));
            _substrate.gameObject.SetActive(false);
        }
    }
}
