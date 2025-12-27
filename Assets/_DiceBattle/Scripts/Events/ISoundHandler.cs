using GameSignals;

namespace DiceBattle.Audio
{
    public interface ISoundHandler : IGlobalSubscriber
    {
        void PlaySound(SoundType soundType);
        void SetMusicValue(float value);
        void SetSoundValue(float value);
    }
}
