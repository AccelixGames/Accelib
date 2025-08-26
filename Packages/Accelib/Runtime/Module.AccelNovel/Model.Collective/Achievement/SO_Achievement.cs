using UnityEngine;

namespace Accelib.Module.AccelNovel.Model.Collective.Achievement
{
    [CreateAssetMenu(fileName = "Ach_00", menuName = "Accelib/Achievement", order = 0)]
    public class SO_Achievement : ScriptableObject
    {
        [SerializeField, TextArea] private string memo;
        
        [field:Header("플랫폼별 키값")]
        [field: SerializeField] public string SteamKey { get; private set; }
        [field: SerializeField] public string GooglePlayKey { get; private set; }
        [field: SerializeField] public string AppStoreKey { get; private set; }
        [field: SerializeField] public string StoveKey { get; private set; }

        public bool CheckAndAchieve()
        {
            var key =
#if UNITY_STANDALONE && STEAMWORKS_NET && !DISABLESTEAMWORKS
                SteamKey;
#elif UNITY_ANDROID
                GooglePlayKey;
#elif UNITY_IOS
                AppStoreKey;
#elif STOVE_BUILD
                StoveKey;
#else
                "";
#endif

            if (string.IsNullOrEmpty(key))
                return false;
            
            // Deb.Log($"도전과제 달성 시도: {name}({key}) - {memo}", this);
            return AchievementSingleton.Instance?.CheckAndAchieve(key) ?? false;
        }
    }
}