using DiceBattle.Audio;
using GameSignals;

namespace DiceBattle.Events
{
    public interface ISoundHandler : IGlobalSubscriber
    {
        void PlayMusic(SoundType soundType);
        void PlaySound(SoundType soundType);
        void SetMusicValue(float value);
        void SetSoundValue(float value);
    }
}
