﻿
using UnityEngine;

namespace Accelib.Module.GameConfig
{
#if UNITY_STANDALONE && STEAMWORKS_NET && !DISABLESTEAMWORKS
    [CreateAssetMenu(fileName = nameof(SteamConfig), menuName = "Accelib/Configs/" + nameof(SteamConfig), order = 0)]
    public class SteamConfig : ScriptableObject
    {
        [SerializeField] private long appId;
        [SerializeField] private long appIdDemo;

        public uint AppId => 
#if !DEMO_BUILD
            (uint)appId;
#else
            (uint)appIdDemo;
#endif
        
        public static SteamConfig Load() => Resources.Load<SteamConfig>(nameof(Accelib) + "/" + nameof(SteamConfig));
    }
#endif
}