using Cysharp.Threading.Tasks;

namespace Accelib.Module.SceneManagement
{
    public interface ISceneChangedEventHandler
    {
        public UniTask OnAfterSceneChanged();
    }
}