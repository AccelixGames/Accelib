#if UNITY_SWITCH
using Accelib.Module.SaveLoad.RemoteStorage.Base;
using Accelib.Module.SaveLoad.RemoteStorage.Data;
using Cysharp.Threading.Tasks;

namespace Accelib.Module.SaveLoad.RemoteStorage
{
    public class RemoteStorage_Switch : IRemoteStorage
    {
        public string Name => "Switch";

        public string GetFilePath(string fileName)
        {
            return "";
        }

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