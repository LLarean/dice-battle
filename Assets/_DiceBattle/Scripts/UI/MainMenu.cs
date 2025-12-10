using DiceBattle.Audio;
using GameSignals;
using UnityEngine;
using UnityEngine.UI;

namespace DiceBattle
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] private Button _start;

        private void Start() => _start.onClick.AddListener(StartClick);

        private void OnDestroy() => _start.onClick.RemoveAllListeners();

        private void StartClick()
        {
            SignalSystem.Raise<ISoundHandler>(handler => handler.PlaySound(SoundType.Click));
            SignalSystem.Raise<IScreenHandler>(handler => handler.ShowScreen(ScreenType.GameScreen));
            gameObject.SetActive(false);
        }
    }
}
