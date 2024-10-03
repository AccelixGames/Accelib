using System;
using Accelib.Audio.Data._Base;
using Accelib.Logging;
using UnityEngine;

namespace Accelib.Audio.Data
{
    [CreateAssetMenu(fileName = "(AudioRef) Name", menuName = "Accelib/AudioRef", order = 0)]
    public class AudioRefSO : ScriptableObject, IAudioRef
    {
        [field: Header("Clip")]
        [field: SerializeField] public AudioChannel Channel { get; private set; }

        [field: SerializeField] public AudioClip Clip { get; private set; }

        [field: Header("Option")]
        [field: SerializeField] [field: Range(0f, 1f)]
        public float Volume { get; private set; } = 1f;

        [field: SerializeField]
        public bool Loop { get; private set; } = false;

        private bool Validate() => Clip != null;
        
        public void Play()
        {
            if (!Validate())
            {
                Deb.LogWarning($"Invalid AudioRef: {name}", this);
                return;
            }

            AudioSingleton.Play(this);
        }

        public void PlayOneShot()
        {
            if (!Validate())
            {
                Deb.LogWarning($"Invalid AudioRef: {name}", this);
                return;
            }

            AudioSingleton.PlayOneShot(this);
        }

#if UNITY_EDITOR
        public static AudioRefSO CreateAssetFromClip(AudioClip clip, string folderPath, bool autoSave = false)
        {
            if (clip == null) return null;

            try
            {
                var audioRef = CreateInstance<AudioRefSO>();
                var name = clip.name.ToLowerInvariant();

                audioRef.Clip = clip;
                audioRef.name = $"(AudioRef) {clip.name}";
                var filePath = System.IO.Path.Combine(folderPath, audioRef.name + ".asset");

                if (name.Contains("bgm"))
                    audioRef.Channel = AudioChannel.Bgm;
                else if (name.Contains("ambient"))
                    audioRef.Channel = AudioChannel.Ambient;
                else if (name.Contains("voice"))
                    audioRef.Channel = AudioChannel.Sfx_Voice;
                else if (name.Contains("sfx_ui"))
                    audioRef.Channel = AudioChannel.Sfx_UI;
                else if (name.Contains("sfx"))
                    audioRef.Channel = AudioChannel.Sfx_Effect;

                // 파일 생성
                UnityEditor.AssetDatabase.CreateAsset(audioRef, filePath);

                // 더티 처리
                UnityEditor.EditorUtility.SetDirty(audioRef);

                // 저장
                if (autoSave)
                {
                    UnityEditor.AssetDatabase.SaveAssets();
                    UnityEditor.AssetDatabase.Refresh();   
                }

                return audioRef;
            }
            catch (Exception e)
            {
                Deb.LogException(e);
                return null;
            }
        }
#endif
    }
}