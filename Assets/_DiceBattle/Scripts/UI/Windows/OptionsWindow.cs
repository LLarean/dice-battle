using DiceBattle.Events;
using DiceBattle.Global;
using GameSignals;
using TMPro;
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
        [SerializeField] private Button _language;
        [SerializeField] private Button _close;
        [Space]
        [SerializeField] private TMP_Text _version;

        private void Start()
        {
            _music.onValueChanged.AddListener(HandleMusicChange);
            _sound.onValueChanged.AddListener(HandleSoundChange);

            _credits.onClick.AddListener(HandleCreditsClick);
            _language.onClick.AddListener(HandleLanguageClick);
            _close.onClick.AddListener(HandleCloseClick);

            _version.text = Application.version;
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
            SignalSystem.Raise<ISoundHandler>(handler => handler.SetMusicVolume(musicValue));
        }

        private void HandleSoundChange(float soundValue)
        {
            SignalSystem.Raise<ISoundHandler>(handler => handler.SetSoundVolume(soundValue));
        }

        private void HandleCreditsClick()
        {
            SignalSystem.Raise<IScreenHandler>(handler => handler.ShowWindow(ScreenType.CreditsWindow));
            gameObject.SetActive(false);
        }

        private void HandleLanguageClick()
        {
            Debug.Log("Language change");
        }

        private void HandleCloseClick() => gameObject.SetActive(false);
    }
}
