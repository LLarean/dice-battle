using DiceBattle.Events;
using DiceBattle.Global;
using GameSignals;
using UnityEngine;
using UnityEngine.UI;

namespace DiceBattle.UI
{
    public class OptionsWindow : Screen
    {
        [Space]
        [SerializeField] private Slider _music;
        [SerializeField] private Slider _sound;
        [Space]
        [SerializeField] private Button _credits;
        [SerializeField] private Button _close;

        private void Start()
        {
            _music.onValueChanged.AddListener(HandleMusicChange);
            _sound.onValueChanged.AddListener(HandleSoundChange);

            _credits.onClick.AddListener(HandleCreditsClick);
            _close.onClick.AddListener(HandleCloseClick);
        }

        private void OnDestroy()
        {
            _music.onValueChanged.RemoveAllListeners();
            _sound.onValueChanged.RemoveAllListeners();

            _credits.onClick.RemoveAllListeners();
            _close.onClick.RemoveAllListeners();
        }

        private void OnEnable()
        {
            _music.value = GameSettings.MusicVolume;
            _sound.value = GameSettings.SoundVolume;
        }

        private void HandleMusicChange(float musicValue)
        {
            SignalSystem.Raise<ISoundHandler>(handler => handler.SetMusicValue(musicValue));
        }

        private void HandleSoundChange(float soundValue)
        {
            SignalSystem.Raise<ISoundHandler>(handler => handler.SetSoundValue(soundValue));
        }

        private void HandleCreditsClick()
        {
            Debug.Log("Credits");
            gameObject.SetActive(false);
        }

        private void HandleCloseClick() => gameObject.SetActive(false);
    }
}
