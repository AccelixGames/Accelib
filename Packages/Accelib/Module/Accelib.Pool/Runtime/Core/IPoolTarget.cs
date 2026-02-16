namespace Accelib.Pool
{
    /// <summary>
    /// 풀 대상 오브젝트가 구현하는 인터페이스. 풀에 반환될 때 OnRelease가 호출된다.
    /// </summary>
    public interface IPoolTarget
    {
        public void OnRelease() {}
    }
}
