using System;
using Accelib.Conditional.Data;
using Accelib.Conditional.Utility;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Accelib.Conditional
{
    /// <summary> 단일 조건식. 좌변과 우변을 비교 연산자로 비교하며, 논리 연산자로 다음 조건과 연결된다 </summary>
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

        /// <summary> 조건을 평가한다 </summary>
        public bool Evaluate() => lhs.CompareTo(rhs, comparisonOperator);

        public string Preview => $"{lhs.Preview} {comparisonOperator.ToStringSign()} {rhs.Preview}";

        /// <summary> 좌변 값 변경 구독 </summary>
        public IDisposable SubscribeLhs(Action<double> onChanged) => lhs.Subscribe(onChanged);
        /// <summary> 우변 값 변경 구독 </summary>
        public IDisposable SubscribeRhs(Action<double> onChanged) => rhs.Subscribe(onChanged);
    }
}
