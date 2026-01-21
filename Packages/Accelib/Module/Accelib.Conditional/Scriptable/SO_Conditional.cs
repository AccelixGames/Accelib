using System.Collections.Generic;
using Accelib.Conditional.Definition;
using Accelib.Conditional.Model;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Accelib.Conditional.Scriptable
{
    [CreateAssetMenu(fileName = "(Conditional) Name", menuName = "Accelib/Conditional/Conditional", order = 0)]
    public class SO_Conditional : ScriptableObject
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
    }
}