using Accelib.Module.AccelNovel.Maid;
using UnityEngine;

namespace Accelib.Module.AccelNovel.Maid
{
    [CreateAssetMenu(fileName = "SO_MaidStartScenario", menuName = "Maid_Scenario/Scenario/SO_MaidStartScenario")]
    public class SO_MaidStartScenario : SO_MaidScenarioBase
    {
        [Header("대화 오픈 조건")]
        [field:SerializeField] public SStartFilter SFilter;
    }
}
