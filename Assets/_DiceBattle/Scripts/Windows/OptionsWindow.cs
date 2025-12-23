using DiceBattle.Audio;
using DiceBattle.Global;
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
            _music.value = PlayerPrefs.GetFloat(PlayerPrefsKeys.SoundVolume, 1);
            _sound.value = PlayerPrefs.GetFloat(PlayerPrefsKeys.SoundVolume, 1);
        }

        private void HandleCloseClick() => gameObject.SetActive(false);

        private void HandleMusicChange(float musicValue)
        {
            SignalSystem.Raise<ISoundHandler>(handler => handler.ChangeMusicValue(musicValue));
        }

        private void HandleSoundChange(float soundValue)
        {
            SignalSystem.Raise<ISoundHandler>(handler => handler.ChangeSoundValue(soundValue));
        }
    }
}
