#if UNITY_IOS
using System.IO;
using UnityEngine;

namespace Accelib.Module.SaveLoad.RemoteStorage
{
    public class RemoteStorage_IOS : RemoteStorage_Local
    {
        public override string Name => "IOS";

        public override string GetFilePath(string fileName) => Path.Combine(Application.persistentDataPath   , "..", "..", "Documents", fileName);
    }
}
#endif