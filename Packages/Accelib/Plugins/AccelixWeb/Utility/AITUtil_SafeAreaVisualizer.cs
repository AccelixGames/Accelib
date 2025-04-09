#if ACCELIB_AIT
using System;
using Accelix.Plugins.AccelixWeb;
using TMPro;
using UnityEngine;

namespace Accelib.AccelixWeb.Utility
{
    public class AITUtil_SafeAreaVisualizer : MonoBehaviour
    {
        [SerializeField] private TMP_Text tmp;

        private void Update()
        {
            var area = AppInTossNative.GetSafeArea();
            tmp.text = $"top: {area.top}, bottom: {area.bottom}";
        }
    }
}
#endif