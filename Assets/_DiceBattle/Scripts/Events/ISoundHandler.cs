using GameSignals;

namespace DiceBattle.Audio
{
    public interface ISoundHandler : IGlobalSubscriber
    {
        void PlaySound(SoundType soundType);
        void ChangeMusicValue(float value);
        void ChangeSoundValue(float value);
    }
}
