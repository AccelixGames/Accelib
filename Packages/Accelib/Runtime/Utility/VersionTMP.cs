using System;
using TMPro;
using UnityEngine;

namespace Accelib.Utility
{
    public class VersionTMP : MonoBehaviour
    {
        [SerializeField] private TMP_Text text;

        private void OnEnable()
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            const string devStr = "d";
#else
            const string devStr = "p";
#endif
            var platform = Application.platform.ToString();
            
            // v0522.1(standalone64x-prod)
            text.text = $"{devStr}{Application.version}({platform})";
        }

        private void Reset()
        {
            text = GetComponent<TMP_Text>();
        }
        
#if UNITY_EDITOR
        private void OnValidate()
        {
            if (Application.isPlaying) return;
            
            text ??= GetComponent<TMP_Text>();
            OnEnable();
        }
#endif
    }
}