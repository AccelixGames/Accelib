using UnityEngine;

namespace Accelib.Module.AccelNovel.Maid
{
    [CreateAssetMenu(fileName = "SO_MaidDailyScenario", menuName = "Scriptable Objects/SO_MaidDailyScenario")]
    public class SO_MaidDailyScenario : SO_MaidScenarioBase
    {
        [field:Header("대화 오픈 조건")]
        [field: SerializeField] public SDailyFilter dFilter;
    }

    
}
