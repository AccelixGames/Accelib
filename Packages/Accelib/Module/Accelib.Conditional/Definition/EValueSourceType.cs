using UnityEngine;

namespace Accelib.Conditional.Definition
{
    public enum EValueSourceType
    {
        Integer = 0,
        Double = 1,
        Boolean = 2,
        [InspectorName("Preset")] ScriptableObject = 10,
        [InspectorName("Custom")] Custom = 11,
    }
}