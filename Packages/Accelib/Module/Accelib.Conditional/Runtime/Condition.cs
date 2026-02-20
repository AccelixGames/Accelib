using System;
using System.Collections.Generic;
using System.Linq;
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
        public bool Evaluate()
        {
            if (comparisonOperator == EComparisonOperator.Contains)
                return EvaluateContains();
            return lhs.CompareTo(rhs, comparisonOperator);
        }

        /// <summary> LHS 컬렉션이 RHS 문자열을 포함하는지 평가한다 </summary>
        private bool EvaluateContains()
        {
            var collection = lhs.GetRawValue();
            var search = rhs.StringValue;
            if (string.IsNullOrEmpty(search) || collection == null) return false;

            return collection switch
            {
                ICollection<string> set => set.Contains(search),
                IEnumerable<string> enumerable => enumerable.Contains(search),
                _ => false
            };
        }

        public string Preview => $"{lhs.Preview} {comparisonOperator.ToStringSign()} {rhs.Preview}";

        /// <summary> 좌변 값 변경 구독 </summary>
        public IDisposable SubscribeLhs(Action<double> onChanged) => lhs.Subscribe(onChanged);
        /// <summary> 우변 값 변경 구독 </summary>
        public IDisposable SubscribeRhs(Action<double> onChanged) => rhs.Subscribe(onChanged);
    }
}
