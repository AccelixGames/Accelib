using R3;

namespace Accelib.R3Extension.Runtime
{
    /// <summary>
    /// R3 Observable 확장 메서드 모음.
    /// 자주 사용하는 연산자 조합을 간결한 메서드로 제공한다.
    /// </summary>
    public static class ObservableExtension
    {
        /// <summary>
        /// 연속된 두 값의 차이(델타)를 발행한다.
        /// 내부: Skip(1) → DistinctUntilChanged() → Pairwise() → Select(Current - Previous)
        /// </summary>
        /// <param name="source">원본 Observable&lt;int&gt;</param>
        /// <returns>이전 값과 현재 값의 차이를 발행하는 Observable&lt;int&gt;</returns>
        public static Observable<int> Delta(this Observable<int> source)
        {
            return source
                .DistinctUntilChanged()
                .Pairwise()
                .Select(p => p.Current - p.Previous);
        }
    }
}
