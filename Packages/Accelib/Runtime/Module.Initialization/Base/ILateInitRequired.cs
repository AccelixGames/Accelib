namespace Accelib.Module.Initialization.Base
{
    public interface ILateInitRequired
    {
        public int Priority { get; }
        public void Init();
        public bool IsInitialized();
    }
}