using System;
using System.Globalization;
using Accelib.Conditional.Data;
using Accelib.Reflection;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Accelib.Conditional
{
    /// <summary> 값 제공자. 리터럴, ScriptableObject, MemberRef 등 다양한 소스에서 double 값을 제공한다 </summary>
    [Serializable]
    public struct ValueProvider
    {
        [HideLabel][SerializeField] private EValueSourceType sourceType;

        [HideLabel][SerializeField, ShowIf("sourceType", EValueSourceType.Boolean)]  private bool booleanValue;
        [HideLabel][SerializeField, ShowIf("sourceType", EValueSourceType.Integer)]  private int intValue;
        [HideLabel][SerializeField, ShowIf("sourceType", EValueSourceType.Double)]  private double doubleValue;

        [HideLabel][SerializeField, ShowIf("sourceType", EValueSourceType.ScriptableObject), HorizontalGroup]
        private SO_ValueProviderBase soValue;

        [HideLabel][SerializeField, ShowIf("sourceType", EValueSourceType.Custom), HorizontalGroup]
        private MemberRef customValue;

        [HideLabel][ShowInInspector, ShowIf("sourceType", EValueSourceType.ScriptableObject), HorizontalGroup(width:0.3f)]
        public double Value => sourceType switch
        {
            EValueSourceType.Boolean => booleanValue ? 1 : 0,
            EValueSourceType.Integer => intValue,
            EValueSourceType.Double => doubleValue,
            EValueSourceType.ScriptableObject => soValue?.Value ?? 0,
            EValueSourceType.Custom => customValue?.Value ?? 0,
            _ => 0
        };

        public string Preview => sourceType switch
        {
            EValueSourceType.Boolean => booleanValue ?  "true" : "false",
            EValueSourceType.Custom => $"'{customValue?.GetPreview()}'[{Value}]",
            EValueSourceType.ScriptableObject => $"'{soValue?.Preview}'[{Value}]",
            _ => Value.ToString(CultureInfo.InvariantCulture)
        };

        /// <summary> 다른 ValueProvider와 비교한다 </summary>
        public bool CompareTo(ValueProvider other, EComparisonOperator oper)
        {
            const double epsilon = double.Epsilon;

            var value = Value;
            var otherValue = other.Value;
            var diff = value - otherValue;
            var equal = Math.Abs(diff) <= epsilon;

            return oper switch
            {
                EComparisonOperator.Equal => equal,
                EComparisonOperator.NotEqual => !equal,
                EComparisonOperator.GreaterThan => value > otherValue,
                EComparisonOperator.LessThan => value < otherValue,
                EComparisonOperator.GreaterThanOrEqual => value > otherValue || equal,
                EComparisonOperator.LessThanOrEqual => value < otherValue || equal,
                _ => false
            };
        }
    }
}
