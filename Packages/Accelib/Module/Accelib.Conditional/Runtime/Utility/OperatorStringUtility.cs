using Accelib.Conditional.Data;

namespace Accelib.Conditional.Utility
{
    /// <summary> 비교 연산자 문자열 변환 유틸리티 </summary>
    public static class OperatorStringUtility
    {
        /// <summary> 비교 연산자를 문자열 기호로 변환한다 </summary>
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
