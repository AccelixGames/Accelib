using System;
using NaughtyAttributes;
using TMPro;
using UnityEngine;

namespace Accelib.Module.UI.Utility
{
    [RequireComponent(typeof(TMP_Text))]
    public class VersionText : MonoBehaviour
    {
        [SerializeField, ReadOnly] private string devStr;
        [SerializeField, ReadOnly] private string version;
        
        private TMP_Text _target;

        private void OnEnable()
        {
            #if DEVELOPMENT_BUILD
            devStr = "dev";
            #else
            devStr = "prod";
            #endif

            version = Application.version;
            
            _target = GetComponent<TMP_Text>();
            _target.text = devStr + "-" + version;
        }
    }
}