using UnityEngine;

namespace Accelib.Module.AccelNovel.Maid
{
    [CreateAssetMenu(fileName = "SO_MaidGiftScenario", menuName = "Maid_Scenario/Scenario/SO_MaidGiftScenario")]
    public class SO_MaidGiftScenario : SO_MaidScenarioBase
    {
        [Header("대화 오픈 조건")]
        [field:SerializeField] public SGiftFilter GFilter;
    }
}
