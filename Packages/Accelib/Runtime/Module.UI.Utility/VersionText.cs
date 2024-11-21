using System;
using NaughtyAttributes;
using TMPro;
using UnityEngine;

namespace Accelib.Module.UI.Utility
{
    [RequireComponent(typeof(TMP_Text))]
    public class VersionText : MonoBehaviour
    {
        [SerializeField, ReadOnly] private string version;
        [SerializeField, ReadOnly] private string devStr;
        
        private TMP_Text _target;

        private void OnEnable()
        {
            var demoStr = "";
#if DEMO_BUILD || UNITY_ANDROID || UNITY_IOS
            demoStr = "demo-";
#endif
            
#if DEVELOPMENT_BUILD
            devStr = "dev";
#else
            devStr = "prod";
#endif

            version = Application.version;
            
            _target = GetComponent<TMP_Text>();
            _target.text = $"{demoStr}{version}-{devStr}";
        }
    }
}