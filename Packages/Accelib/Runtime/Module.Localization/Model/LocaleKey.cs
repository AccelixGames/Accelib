using System.Collections.Generic;
using UnityEngine;

namespace Accelib.Module.Localization.Model
{
    [System.Serializable]
    public class LocaleKey
    {
        [SerializeField] private string key;

        public string Key => key;
        public void SetKey(string value) => key = value;
        
#if UNITY_EDITOR
        [SerializeField, HideInInspector] 
        private string[] preview;
#endif
    }
}