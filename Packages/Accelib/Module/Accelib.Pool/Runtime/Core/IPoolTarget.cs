namespace Accelib.Pool
{
    /// <summary>
    /// 풀 대상 오브젝트가 구현하는 인터페이스. 풀에 반환될 때 OnRelease가 호출된다.
    /// </summary>
    public interface IPoolTarget
    {
        /// <summary>풀에 반환될 때 호출된다. 상태 초기화 용도로 오버라이드한다.</summary>
        public void OnRelease() {}
    }
}
