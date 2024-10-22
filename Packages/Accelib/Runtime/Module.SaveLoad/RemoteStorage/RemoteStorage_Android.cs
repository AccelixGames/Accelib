#if UNITY_ANDROID
namespace Accelib.Module.SaveLoad.RemoteStorage
{
    public class RemoteStorage_Android : RemoteStorage_Local
    {
        public override string Name => "Android";
    }
}
#endif