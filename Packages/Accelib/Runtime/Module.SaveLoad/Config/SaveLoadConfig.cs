using System;
using NaughtyAttributes;
using UnityEngine;

namespace Accelib.Module.SaveLoad.Config
{
    [CreateAssetMenu(fileName = nameof(SaveLoadConfig), menuName = "Accelib/Configs/" + nameof(SaveLoadConfig), order = 0)]
    public sealed class SaveLoadConfig : ScriptableObject
    {
        [field: Header("암호화 옵션")]
        [field: SerializeField] public string Secret { get; private set; } = "di@94n)fglwi2l#K";
        [SerializeField, ReadOnly] private byte[] secretBytes;

        [field: Header("강제 옵션")]
        [field: SerializeField] public bool ForceLocalStorage
#if UNITY_EDITOR 
        { get; private set; } = false;
#else
        => false;
#endif
        [field: SerializeField] public bool ForceNoWrite { get; private set; }
        [field: SerializeField] public bool ForceNoRead { get; private set; }

        [field: Header("디버그 옵션")]
#if UNITY_EDITOR || DEVELOPMENT_BUILD || ENABLE_DEBUG
        [field: SerializeField] public bool PrintLog { get; private set; } = false;
#else
        public bool PrintLog => false;
#endif

        public static SaveLoadConfig Load() =>
            Resources.Load<SaveLoadConfig>(nameof(Accelib) + "/" + nameof(SaveLoadConfig));
        
#if UNITY_EDITOR
        private void OnValidate()
        {
            var keyArray = System.Text.Encoding.UTF8.GetBytes(Secret);
            if (keyArray.Length >= 16) keyArray = keyArray[..16];
            secretBytes = new byte[keyArray.Length];
            Array.Copy(keyArray, 0, secretBytes, 0, keyArray.Length);
        }
#endif
    }
}