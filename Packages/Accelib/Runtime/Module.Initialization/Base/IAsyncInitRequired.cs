using Cysharp.Threading.Tasks;

namespace Accelib.Module.Initialization.Base
{
    public interface IAsyncInitRequired
    {
        public UniTask<bool> InitAsync();
        
    }
}