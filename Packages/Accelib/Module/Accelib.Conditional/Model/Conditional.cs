using System.Collections.Generic;
using Accelib.Conditional.Definition;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Accelib.Conditional.Model
{
    [System.Serializable]
    public class Conditional
    {
        [ListDrawerSettings(DraggableItems = true, ShowFoldout = false, ShowIndexLabels = false, ShowPaging = false, ShowItemCount = false, HideRemoveButton = false)]
        [SerializeField] private List<Condition> conditions;

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
    }
}