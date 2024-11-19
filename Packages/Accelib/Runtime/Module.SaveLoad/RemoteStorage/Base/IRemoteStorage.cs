using Accelib.Module.SaveLoad.RemoteStorage.Data;
using Cysharp.Threading.Tasks;
using DG.Tweening.Plugins.Core.PathCore;
using UnityEngine;
using Path = System.IO.Path;

namespace Accelib.Module.SaveLoad.RemoteStorage.Base
{
    public interface IRemoteStorage
    {
        public string Name { get; }
        public string GetFilePath(string fileName);
        public UniTask<RemoteTaskResult> WriteAsync(byte[] bytes, string fileName);
        public UniTask<RemoteTaskResult> ReadAsync(string fileName);
        
    }
}