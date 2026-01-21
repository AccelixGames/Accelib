using UnityEngine;

namespace Accelib.Conditional.Definition
{
    /// <summary> 비교 연산자 </summary>
    public enum EComparisonOperator
    {
        [InspectorName("==")] Equal = 0,
        [InspectorName("!=")] NotEqual = 1,
        [InspectorName(">")]  GreaterThan = 2,
        [InspectorName(">=")] GreaterThanOrEqual = 3,
        [InspectorName("<")]  LessThan = 4,
        [InspectorName("<=")] LessThanOrEqual = 5,
    }
}