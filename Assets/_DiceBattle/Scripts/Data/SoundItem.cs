using System.Collections.Generic;
using DiceBattle.Audio;
using UnityEngine;

namespace DiceBattle.Data
{
    [System.Serializable]
    public class SoundItem
    {
        public SoundType SoundType;
        public List<AudioClip> AudioClips = new();
    }
}
