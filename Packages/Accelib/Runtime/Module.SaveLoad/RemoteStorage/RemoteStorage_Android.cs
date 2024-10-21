#if UNITY_ANDROID
using Accelib.Module.SaveLoad.RemoteStorage.Base;
using Accelib.Module.SaveLoad.RemoteStorage.Data;
using Cysharp.Threading.Tasks;

namespace Accelib.Module.SaveLoad.RemoteStorage
{
    public class RemoteStorage_Android : IRemoteStorage
    {
        public string Name => "Android";

        public UniTask<RemoteTaskResult> WriteAsync(byte[] data, string path)
        {
            throw new System.NotImplementedException();
        }

        public UniTask<RemoteTaskResult> ReadAsync(string path)
        {
            throw new System.NotImplementedException();
        }
    }
}
#endif