using System.Collections;
using Accelib.Conditional.Utility;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Accelib.Conditional.Scriptable.Implements
{
    [CreateAssetMenu(fileName = "(Value) Name", menuName = "Accelib/Conditional/Provider-SO", order = 1)]
    public class SO_ValueProviderScriptable : SO_ValueProviderBase
    {
        [SerializeField] private ScriptableObject target;
        [ValueDropdown("GetFields")]
        [SerializeField] private SerializedFieldInfo field;

        public override string Preview => $"{Value:F2}";
        public override double Value => field?.Value ?? 0;

#if UNITY_EDITOR
        private IEnumerable GetFields() => target?.GetSerializedFields();
#endif
    }
}