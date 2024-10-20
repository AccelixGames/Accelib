#if UNITY_STANDALONE && STEAMWORKS_NET && !DISABLESTEAMWORKS
using Accelib.Module.SaveLoad.RemoteStorage.Base;
using Accelib.Module.SaveLoad.RemoteStorage.Data;
using Cysharp.Threading.Tasks;

namespace Accelib.Module.SaveLoad.RemoteStorage
{
    internal class RemoteStorage_Steam : IRemoteStorage
    {
        public string Name => "Steam";
        
        public UniTask<RemoteTaskResult> WriteAsync(byte[] bytes, string relativePath)
        {
            throw new System.NotImplementedException();
        }

        public UniTask<RemoteTaskResult> ReadAsync(string relativePath)
        {
            throw new System.NotImplementedException();
        }
    }
}
#endif