using System.IO;
using Accelib.Module.SaveLoad.RemoteStorage.Base;
using Accelib.Module.SaveLoad.RemoteStorage.Data;
using Cysharp.Threading.Tasks;
using UnityEngine.Device;
using Application = UnityEngine.Application;

namespace Accelib.Module.SaveLoad.RemoteStorage
{
    public class RemoteStorage_Local : IRemoteStorage
    {
        public virtual string Name => "Local";
#if UNITY_EDITOR
        public virtual string GetFilePath(string fileName) => Path.Combine(Application.persistentDataPath, "SavesDir", "Editor", fileName);
#else
        public virtual string GetFilePath(string fileName) => Path.Combine(Application.persistentDataPath, "SavesDir", "Local", fileName);
#endif

        public async UniTask<RemoteTaskResult> WriteAsync(byte[] bytes, string fileName)
        {
            var filePath = GetFilePath(fileName);
            var directory = Path.GetDirectoryName(filePath);
            
            if(!Directory.Exists(directory))
                Directory.CreateDirectory(directory);
            
            await File.WriteAllBytesAsync(filePath, bytes);
            return new RemoteTaskResult(true, bytes);
        }

        public async UniTask<RemoteTaskResult> ReadAsync(string fileName)
        {
            var path = GetFilePath(fileName);
            if (!File.Exists(path)) return new RemoteTaskResult(true, null);
            
            var bytes = await File.ReadAllBytesAsync(path);
            return new RemoteTaskResult(true, bytes);

        }
    }
}