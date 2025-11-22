using NaughtyAttributes;
using UnityEngine;

namespace Accelib.Utility
{
    public class SimpleTimescale : MonoBehaviour
    {
        [SerializeField, Range(0f, 5f)] private float targetTimescale = 1f;
        [ShowNativeProperty] private float CurrTimescale => Time.timeScale;
        
        [Button(enabledMode: EButtonEnableMode.Playmode)]
        public void SetTimescale() => Time.timeScale = targetTimescale;
    }
}