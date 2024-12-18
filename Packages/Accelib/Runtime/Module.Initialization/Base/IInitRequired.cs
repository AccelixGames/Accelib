

namespace Accelib.Module.Initialization.Base
{
    public interface IInitRequired
    {
        public void Init();

        public bool IsInitialized();
    }
}