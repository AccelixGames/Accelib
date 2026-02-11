using Accelib.Effect;
using Accelib.Module.UI.InfoBox.Default.Model;
using UnityEngine;

namespace Accelib.Module.UI.InfoBox.Default.View
{
    public class InfoView_DefaultFade : InfoView_Default
    {
        [SerializeField] private SimpleFadeEffect fadeEffect;

        protected override void OnReceiveInfo(InfoData_Default info)
        {
            if (string.IsNullOrEmpty(info?.description)) 
                fadeEffect.FadeOut(false);
            else
            {
                if(!fadeEffect.gameObject.activeSelf) 
                    fadeEffect.gameObject.SetActive(true);
                fadeEffect.FadeIn(false);
            }
        }
    }
}