using DiceBattle.Audio;
using GameSignals;
using UnityEngine;
using UnityEngine.UI;

namespace DiceBattle.Windows
{
    public class OptionsWindow : MonoBehaviour
    {
        [SerializeField] private Button _close;
        [SerializeField] private Slider _music;
        [SerializeField] private Slider _sound;

        private void Start()
        {
            _close.onClick.AddListener(HandleCloseClick);
            _music.onValueChanged.AddListener(HandleMusicChange);
            _sound.onValueChanged.AddListener(HandleSoundChange);
        }

        private void OnDestroy()
        {
            _close.onClick.RemoveAllListeners();
            _music.onValueChanged.RemoveAllListeners();
            _sound.onValueChanged.RemoveAllListeners();
        }

        private void OnEnable()
        {
            _music.value = GameSettings.MusicVolume;
            _sound.value = GameSettings.SoundVolume;
        }

        private void HandleCloseClick() => gameObject.SetActive(false);

        private void HandleMusicChange(float musicValue)
        {
            SignalSystem.Raise<ISoundHandler>(handler => handler.SetMusicValue(musicValue));
        }

        private void HandleSoundChange(float soundValue)
        {
            SignalSystem.Raise<ISoundHandler>(handler => handler.SetSoundValue(soundValue));
        }
    }
}
