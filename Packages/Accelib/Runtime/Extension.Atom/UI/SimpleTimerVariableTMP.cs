using Accelib.Extension.Atom.UI.Base;
using Accelib.Extensions;
using UnityAtoms;
using UnityAtoms.BaseAtoms;
using UnityEngine;

namespace Accelib.Extension.Atom.UI
{
    public class SimpleTimerVariableTMP : SimpleVariableTMP<FloatVariable, float>
    {
        [Header("TimeMode")]
        [SerializeField] private DateTimeExtension.TimeDisplayMode displayMode = DateTimeExtension.TimeDisplayMode.Min;
        
        protected override AtomEvent<float> Changed => variable?.Changed;
        protected override float GetValue => variable?.Value ?? 0f;

        protected override string GetText(float v)
        {
            var seconds = Mathf.FloorToInt(v);
            var txt = DateTimeExtension.SecToTime(seconds, displayMode);
            
            return string.IsNullOrEmpty(format) ? txt : string.Format(format, txt);
        }
    }
}