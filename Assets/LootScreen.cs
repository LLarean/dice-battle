using DiceBattle.Audio;
using GameSignals;
using UnityEngine;
using UnityEngine.UI;

namespace DiceBattle
{
    public class LootScreen : MonoBehaviour
    {
        [SerializeField] private Button _next;

        private void Start() => _next.onClick.AddListener(StartClick);

        private void OnDestroy() => _next.onClick.RemoveAllListeners();

        private void StartClick()
        {
            SignalSystem.Raise<ISoundHandler>(handler => handler.PlaySound(SoundType.Click));

            gameObject.SetActive(false);
        }
    }
}
