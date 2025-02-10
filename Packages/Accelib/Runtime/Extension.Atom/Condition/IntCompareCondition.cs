using System;
using UnityAtoms.BaseAtoms;
using UnityEngine;

namespace Accelib.Extension.Atom.Condition
{
    [CreateAssetMenu(fileName = "IntCondition", menuName = "Unity Atoms/Conditions/Int/Compare", order = 0)]
    public class IntCompareCondition : IntCondition
    {
        private enum Operator {Bigger, BiggerSame, Smaller, SmallerSame, Same}
        
        [SerializeField] private Operator op;
        [SerializeField] private int target;

        public override bool Call(int t) => op switch
        {
            Operator.Bigger => t > target,
            Operator.BiggerSame => t >= target,
            Operator.Smaller => t < target,
            Operator.SmallerSame => t <= target,
            Operator.Same => t == target,
            _ => false
        };
    }
}