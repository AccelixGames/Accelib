using UnityEngine;

namespace Accelib.Module.AccelNovel.Maid
{
    [CreateAssetMenu(fileName = "SO_MaidToyScenario", menuName = "Maid_Scenario/Scenario/SO_MaidToyScenario")]
    public class SO_MaidToyScenario : SO_MaidScenarioBase
    {
        [Header("대화 오픈 조건")]
        [field:SerializeField] public SToyFilter TFilter;
    }
}
