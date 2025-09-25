using UnityEngine;
using UnityEngine.Serialization;

namespace Accelib.Module.AccelNovel.Maid
{
    [CreateAssetMenu(fileName = "SO_MaidFavorScenario", menuName = "Scriptable Objects/SO_MaidFavorScenario")]
    public class SO_MaidFavorScenario : SO_MaidScenarioBase
    {
        [field:Header("대화 오픈 조건")]
        [field: SerializeField] public SFavorFilter FFilter;
    }
}
