namespace Accelib.Module.SaveLoad.RemoteStorage.Base
{
    public static class RemoteStorageSelector
    {
        public static IRemoteStorage GetLocalStorage() => new RemoteStorage_Local();

        public static IRemoteStorage GetRemoteStorage(bool forceLocalStorage = false)
        {
            if (!forceLocalStorage)
            {
#if UNITY_STANDALONE && STEAMWORKS_NET && !DISABLESTEAMWORKS
            return new RemoteStorage_Steam();
#elif UNITY_ANDROID
            return new RemoteStorage_Android();
#elif UNITY_IOS
            return new RemoteStorage_IOS();
#elif UNITY_SWITCH
            return new RemoteStorage_Switch();
#endif   
            }

            return new RemoteStorage_Local();
        }
    }
}