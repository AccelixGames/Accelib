using System.IO;
using System.Threading;
using Accelib.Module.SaveLoad.RemoteStorage.Base;
using Accelib.Module.SaveLoad.RemoteStorage.Data;
using Cysharp.Threading.Tasks;
using UnityEngine.Device;

namespace Accelib.Module.SaveLoad.RemoteStorage
{
    internal class RemoteStorage_Local : IRemoteStorage
    {
        public string Name => "Local";
        private string AbsolutePath(string relativePath) => Path.Combine(Application.persistentDataPath, relativePath); 

        public async UniTask<RemoteTaskResult> WriteAsync(byte[] bytes, string relativePath)
        {
            var filePath = AbsolutePath(relativePath);
            var directory = Path.GetDirectoryName(filePath);
            
            if(!Directory.Exists(directory))
                Directory.CreateDirectory(directory);
            
            // if (!File.Exists(filePath))
            //     File.Create(filePath);
            
            await File.WriteAllBytesAsync(filePath, bytes);
            return new RemoteTaskResult(true, bytes);
          //  return new RemoteTaskResult(false, null);
        }

        public async UniTask<RemoteTaskResult> ReadAsync(string relativePath)
        {
            var path = AbsolutePath(relativePath);
            if (File.Exists(path))
            {
                var bytes = await File.ReadAllBytesAsync(path);
                return new RemoteTaskResult(true, bytes); 
            }

            return new RemoteTaskResult(true, null);
        }
    }
}