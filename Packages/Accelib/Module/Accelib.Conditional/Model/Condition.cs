using Accelib.Conditional.Definition;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Accelib.Conditional.Model
{
    [System.Serializable]
    public struct Condition
    {
        [EnumToggleButtons, PropertySpace(SpaceAfter = 10)]
        [SerializeField, HideLabel] private ELogicalOperator logicalOperator;
        
        [HorizontalGroup(GroupName = "H")]
        [SerializeField, HideLabel] private ValueProvider lhs;
        [HorizontalGroup(GroupName = "H", Width = 0.1f)]
        [SerializeField, HideLabel] private EComparisonOperator comparisonOperator;
        [HorizontalGroup(GroupName = "H")]
        [SerializeField, HideLabel] private ValueProvider rhs;

        internal ELogicalOperator LogicalOperator => logicalOperator;
        public bool Evaluate() => lhs.CompareTo(rhs, comparisonOperator);
    }
}