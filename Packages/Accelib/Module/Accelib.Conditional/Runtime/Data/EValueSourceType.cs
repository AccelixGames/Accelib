using UnityEngine;

namespace Accelib.Conditional.Data
{
    /// <summary> 값 소스 타입 </summary>
    public enum EValueSourceType
    {
        Integer = 0,
        Double = 1,
        Boolean = 2,
        String = 3,
        [InspectorName("Preset")] ScriptableObject = 10,
        [InspectorName("Preset(Advanced)")] Custom = 11,
    }
}
