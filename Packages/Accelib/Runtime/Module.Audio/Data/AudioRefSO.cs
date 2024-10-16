using System;
using Accelib.Logging;
using Accelib.Module.Audio.Data._Base;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Serialization;

namespace Accelib.Module.Audio.Data
{
    [CreateAssetMenu(fileName = "(AudioRef) Name", menuName = "Accelib/AudioRef", order = 0)]
    public class AudioRefSO : AudioRefBase
    {
        [field: Header("Resource")][field:FormerlySerializedAs("<Channel>k__BackingField")]
        [field: SerializeField] public AudioChannel _Channel { get; private set; }

        [field:FormerlySerializedAs("<Clip>k__BackingField")]
        [field: SerializeField] public AudioClip _Clip { get; private set; }

        [field: Header("Option")]
        [field:FormerlySerializedAs("<Volume>k__BackingField")]
        [field: SerializeField] [field: Range(0f, 1f)]
        public float _Volume { get; private set; } = 1f;

        [field: SerializeField]
        [field:FormerlySerializedAs("<Loop>k__BackingField")]
        public bool _Loop { get; private set; } = false;
        
        public override AudioChannel Channel => _Channel;
        public override AudioClip Clip => _Clip;
        public override float Volume => _Volume;
        public override bool Loop => _Loop;
        protected override bool Validate() => _Clip;
        

#if UNITY_EDITOR
                
        [Header("Debug")]
        [SerializeField] private bool showLog = false;
        public override bool ShowLog => showLog;

        public static AudioRefSO CreateAssetFromClip(AudioClip clip, string folderPath, bool autoSave = false)
        {
            if (clip == null) return null;

            try
            {
                var audioRef = CreateInstance<AudioRefSO>();
                var name = clip.name.ToLowerInvariant();

                audioRef._Clip = clip;
                audioRef.name = $"(AudioRef) {clip.name}";
                var filePath = System.IO.Path.Combine(folderPath, audioRef.name + ".asset");

                if (name.Contains("bgm"))
                    audioRef._Channel = AudioChannel.Bgm;
                else if (name.Contains("ambient"))
                    audioRef._Channel = AudioChannel.Ambient;
                else if (name.Contains("voice"))
                    audioRef._Channel = AudioChannel.Sfx_Voice;
                else if (name.Contains("sfx_ui"))
                    audioRef._Channel = AudioChannel.Sfx_UI;
                else if (name.Contains("sfx"))
                    audioRef._Channel = AudioChannel.Sfx_Effect;

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