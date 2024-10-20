using Accelib.Module.SaveLoad.RemoteStorage.Data;
using Cysharp.Threading.Tasks;

namespace Accelib.Module.SaveLoad.RemoteStorage.Base
{
    public interface IRemoteStorage
    {
        public string Name { get; }
        public UniTask<RemoteTaskResult> WriteAsync(byte[] bytes, string relativePath);
        public UniTask<RemoteTaskResult> ReadAsync(string relativePath);
    }
}