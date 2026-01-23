using System;
using Accelib.Conditional.Definition;

namespace Accelib.Conditional.Utility
{
    public static class OperatorStringUtility
    {
        public static string ToStringSign(this EComparisonOperator oper) => oper switch
        {
            EComparisonOperator.Equal => "==",
            EComparisonOperator.NotEqual => "!=",
            EComparisonOperator.GreaterThan => ">",
            EComparisonOperator.GreaterThanOrEqual => ">=",
            EComparisonOperator.LessThan => "<",
            EComparisonOperator.LessThanOrEqual => "<=",
            _ => ""
        };
    }
}