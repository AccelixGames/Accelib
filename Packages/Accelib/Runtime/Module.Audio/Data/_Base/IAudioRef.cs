using UnityEngine;
using UnityEngine.Audio;

namespace Accelib.Module.Audio.Data._Base
{
    public interface IAudioRef
    {
        public AudioChannel Channel { get; }
        public AudioResource Clip { get; }

        public float Volume { get; }
        public bool Loop { get; }
    }
}