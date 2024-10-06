using UnityEngine;

namespace Accelib.Module.Audio.Data._Base
{
    public interface IAudioRef
    {
        public AudioChannel Channel { get; }
        public AudioClip Clip { get; }

        public float Volume { get; }
        public bool Loop { get; }
    }
}