#if UNITY_STANDALONE && STEAMWORKS_NET && !DISABLESTEAMWORKS
using UnityEngine;

namespace Accelib.Module.GameConfig
{
    [CreateAssetMenu(fileName = nameof(SteamConfig), menuName = "Accelib/Configs/" + nameof(SteamConfig), order = 0)]
    public class SteamConfig : ScriptableObject
    {
        [SerializeField] private long appId;
        [SerializeField] private long appIdDemo;

        public uint AppId =>
#if !IS_DEMO
            (uint)appId;
#else
            (uint)appIdDemo;
#endif
        
        public static SteamConfig Load() => Resources.Load<SteamConfig>(nameof(Accelib) + "/" + nameof(SteamConfig));
    }
}
#endif