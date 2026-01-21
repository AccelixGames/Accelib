using System;
using Accelib.Conditional.Definition;
using Accelib.Conditional.Scriptable;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Accelib.Conditional.Model
{
    [Serializable]
    public struct ValueProvider
    {
        [HideLabel][SerializeField] private EValueSourceType sourceType;
        
        [HideLabel][SerializeField, ShowIf("sourceType", EValueSourceType.ScriptableObject), HorizontalGroup] private SO_ValueProviderBase soValue;
        [HideLabel][ShowInInspector, ShowIf("sourceType", EValueSourceType.ScriptableObject), HorizontalGroup(width:0.3f),
        ShowIf("soValue")] private string SoPreview => soValue?.Preview ?? "";
        
        [HideLabel][SerializeField, ShowIf("sourceType", EValueSourceType.Boolean)]  private bool booleanValue;
        [HideLabel][SerializeField, ShowIf("sourceType", EValueSourceType.Integer)]  private int intValue;
        [HideLabel][SerializeField, ShowIf("sourceType", EValueSourceType.Double)]  private double doubleValue;

        private double Value => sourceType switch
        {
            EValueSourceType.ScriptableObject => soValue?.Value ?? 0,
            EValueSourceType.Boolean => booleanValue ? 1 : 0,
            EValueSourceType.Integer => intValue,
            EValueSourceType.Double => doubleValue,
            _ => 0
        };

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