using Accelib.Logging;
using UnityEngine;

namespace Accelib.Module.Audio.Data._Base
{
    public abstract class AudioRefBase : ScriptableObject
    {
        public abstract AudioChannel Channel { get; }
        public abstract AudioClip Clip { get; }

        public abstract float Volume { get; }
        public abstract bool Loop { get; }
        
//#if UNITY_EDITOR
        public abstract bool ShowLog { get; }
//#endif
        
        public virtual void Play(bool fade = false)
        {
            if (!Validate())
            {
                Deb.LogWarning($"Invalid AudioRef: {name}", this);
                return;
            }

            AudioSingleton.Play(this, fade);
            Log($"AudioRef.Play: {name}");
        }

        public void PlayOneShot() => PlayOneShot(0f);

        public void PlayOneShot(float delay)
        {
            if (!Validate())
            {
                Deb.LogWarning($"Invalid AudioRef: {name}", this);
                return;
            }

            AudioSingleton.PlayOneShot(this, delay);
            Log($"AudioRef.PlayOneShot Called: {name}");
        }
        
        public void SwitchFade(bool skipOnSame = true)
        {
            if (!Validate())
            {
                Deb.LogWarning($"Invalid AudioRef: {name}", this);
                return;
            }
            
            AudioSingleton.SwitchFade(this, skipOnSame);
            Log($"AudioRef.SwitchFade: {name}");
        }

        public void Stop(bool fade = false)
        {
            if (!Validate())
            {
                Deb.LogWarning($"Invalid AudioRef: {name}", this);
                return;
            }

            AudioSingleton.Stop(this, fade);
            Log($"AudioRef.Stop: {name}");
        }

        protected abstract bool Validate();
        
        [System.Diagnostics.Conditional("UNITY_EDITOR"), HideInCallstack]
        protected void Log(string msg)
        {
#if UNITY_EDITOR
            if(ShowLog) Deb.Log(msg, this);
#endif
        }
    }
}