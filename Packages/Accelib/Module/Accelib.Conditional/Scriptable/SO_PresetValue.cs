using Accelib.Reflection;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Accelib.Conditional.Scriptable
{
    [CreateAssetMenu(fileName = "(Value) Name", menuName = "Accelib/Conditional/PresetValue", order = 0)]
    public class SO_PresetValue : SO_ValueProviderBase
    {
        [SerializeField, InlineProperty] private MemberRef customValue;

        [ShowInInspector, TextArea]
        public override string Preview => customValue?.GetPreview();
        public override double Value => customValue?.Value ?? 0;
    }
}