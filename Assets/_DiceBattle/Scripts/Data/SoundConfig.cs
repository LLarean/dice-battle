using System.Collections.Generic;
using DiceBattle.Audio;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DiceBattle.Data
{
    [CreateAssetMenu(fileName = "SoundConfig", menuName = "Dice Battle/Sound Config", order = 1)]
    public class SoundConfig : ScriptableObject
    {
        [SerializeField] private List<SoundItem> _soundItems = new();

        public bool TryGetAudioClip(SoundType soundType, out AudioClip audioClip)
        {
            audioClip = null;

            foreach (SoundItem soundItem in _soundItems)
            {
                if (soundItem.SoundType != soundType)
                {
                    continue;
                }

                if (soundItem.AudioClips == null)
                {
                    Debug.LogWarning("soundItem.AudioClips == null");
                    return false;
                }

                if (soundItem.AudioClips.Count == 0)
                {
                    Debug.LogWarning("soundItem.AudioClips.Count == 0");
                    return false;
                }

                audioClip = soundItem.AudioClips[Random.Range(0, soundItem.AudioClips.Count)];

                if (audioClip == null)
                {
                    Debug.LogWarning("audioClip == null");
                    return false;
                }

            }

            return true;
        }
    }
}
