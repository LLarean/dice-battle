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
        [SerializeField] private Button _close;
        [Space]
        [SerializeField] private Slider _music;
        [SerializeField] private Slider _sound;
        [Space]
        [SerializeField] private Button _info;
        [SerializeField] private Button _credits;
        [SerializeField] private Button _share;
        [Space]
        [SerializeField] private TMP_Text _version;

        #region Unity lifecycle

        private void Start()
        {
            _close.onClick.AddListener(HandleCloseClick);

            _music.onValueChanged.AddListener(HandleMusicChange);
            _sound.onValueChanged.AddListener(HandleSoundChange);

            _info.onClick.AddListener(HandleInfoClick);
            _credits.onClick.AddListener(HandleCreditsClick);
            _share.onClick.AddListener(HandleShareClick);

            _version.text = Application.version;
        }

        private void OnDestroy()
        {
            _close.onClick.RemoveAllListeners();

            _music.onValueChanged.RemoveAllListeners();
            _sound.onValueChanged.RemoveAllListeners();

            _info.onClick.RemoveAllListeners();
            _credits.onClick.RemoveAllListeners();
            _share.onClick.RemoveAllListeners();
        }

        private void OnEnable()
        {
            _music.value = GameSettings.MusicVolume;
            _sound.value = GameSettings.SoundVolume;
        }

        #endregion

        #region Handlers

        private void HandleCloseClick()
        {
            SignalSystem.Raise<IScreenHandler>(handler => handler.CloseTopWindow());
        }

        private void HandleMusicChange(float musicValue)
        {
            SignalSystem.Raise<ISoundHandler>(handler => handler.SetMusicVolume(musicValue));
        }

        private void HandleSoundChange(float soundValue)
        {
            SignalSystem.Raise<ISoundHandler>(handler => handler.SetSoundVolume(soundValue));
        }

        private void HandleInfoClick()
        {
            SignalSystem.Raise<IScreenHandler>(handler => handler.CloseTopWindow());
        }

        private void HandleCreditsClick()
        {
            SignalSystem.Raise<IScreenHandler>(handler => handler.CloseTopWindow());
            SignalSystem.Raise<IScreenHandler>(handler => handler.ShowWindow(ScreenType.CreditsWindow));
        }

        private void HandleShareClick()
        {
            SignalSystem.Raise<IScreenHandler>(handler => handler.CloseTopWindow());
        }

        #endregion
    }
}
