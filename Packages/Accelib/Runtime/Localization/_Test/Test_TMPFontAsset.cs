using System;
using System.Linq;
using Accelib.Localization.Architecture;
using TMPro;
using UnityEngine;

namespace Accelib.Localization._Test
{
    public class Test_TMPFontAsset : MonoBehaviour
    {
        [SerializeField] private LocaleSO locale;
        [SerializeField] private TMP_Text text;

        private void Start()
        {
            var mats = text.fontSharedMaterials.ToList();
            var myFont = text.fontMaterial;
            var currId = mats.IndexOf(myFont);
            
            foreach (var material in mats)
            {
                Debug.Log("머테리얼: " + material.name);
            }
            
            Debug.Log($"현재 머테리얼 {myFont.name}({currId})");
            
            //var font = locale.FontAsset;
            
        }
    }
}