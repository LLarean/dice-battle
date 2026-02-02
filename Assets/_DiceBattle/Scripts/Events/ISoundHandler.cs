using DiceBattle.Audio;
using GameSignals;

namespace DiceBattle.Events
{
    public interface ISoundHandler : IGlobalSubscriber
    {
        void PlayMusic(SoundType soundType);
        void PlaySound(SoundType soundType);
        void SetMusicVolume(float value);
        void SetSoundVolume(float value);
    }
}
