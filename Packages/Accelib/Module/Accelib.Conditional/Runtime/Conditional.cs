using System;
using System.Collections.Generic;
using Accelib.Conditional.Data;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Accelib.Conditional
{
    /// <summary> 다중 조건식 컨테이너. 논리 연산자(AND/OR)로 연결된 조건들을 순차 평가한다 </summary>
    [System.Serializable]
    public class Conditional
    {
        [ListDrawerSettings(DraggableItems = true, ShowFoldout = false, ShowIndexLabels = false, ShowPaging = false, ShowItemCount = false, HideRemoveButton = false)]
        [SerializeField] private List<Condition> conditions;

        /// <summary> 전체 조건식을 평가한다 </summary>
        [Button(DirtyOnClick = false, DrawResult = true)]
        public bool Evaluate()
        {
            if (conditions is not { Count: > 0 }) return false;

            var result = conditions[0].Evaluate();

            for (var i = 1; i < conditions.Count; i++)
            {
                var condition = conditions[i];
                var conditionEval = condition.Evaluate();

                if (condition.LogicalOperator == ELogicalOperator.And)
                    result = result && conditionEval;
                else
                    result = result || conditionEval;
            }

            return result;
        }

        /// <summary> 조건 내 모든 ValueProvider의 값 변경을 구독한다. </summary>
        public IDisposable Subscribe(Action onConditionMayChanged)
        {
            if (conditions is not { Count: > 0 }) return null;

            var subs = new List<IDisposable>();
            foreach (var condition in conditions)
            {
                var lhsSub = condition.SubscribeLhs(_ => onConditionMayChanged());
                var rhsSub = condition.SubscribeRhs(_ => onConditionMayChanged());

                if (lhsSub != null) subs.Add(lhsSub);
                if (rhsSub != null) subs.Add(rhsSub);
            }

            return subs.Count > 0 ? new DisposableGroup(subs) : null;
        }

        [ShowInInspector, TextArea, ReadOnly, PropertyOrder(float.MinValue)]
        public string Preview
        {
            get
            {
                if (conditions is not { Count: > 0 }) return string.Empty;

                var result = "";
                for (var i = 0; i < conditions.Count; i++)
                {
                    if (i != 0) result += $"\n{conditions[i].LogicalOperator.ToString().ToUpper()}\n";
                    result += conditions[i].Preview;
                }

                return result;
            }
        }

        /// <summary> 여러 IDisposable을 묶어 한 번에 해제하는 내부 컨테이너 </summary>
        private sealed class DisposableGroup : IDisposable
        {
            private List<IDisposable> _items;
            public DisposableGroup(List<IDisposable> items) => _items = items;
            public void Dispose()
            {
                if (_items == null) return;
                foreach (var d in _items) d?.Dispose();
                _items = null;
            }
        }
    }
}
