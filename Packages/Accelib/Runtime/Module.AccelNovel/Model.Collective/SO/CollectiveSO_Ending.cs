using Accelib.Module.AccelNovel.Model.Collective.SO.Base;
using UnityEngine;

namespace Accelib.Module.AccelNovel.Model.Collective.SO
{
    [CreateAssetMenu(fileName = "Collective-Ending", menuName = "Terri/Collective-Ending", order = 2)]
    public class CollectiveSO_Ending : CollectiveSO_Base
    {
        [field: Header("Detail")]
        [field: SerializeField] public Sprite Image { get; set; }
        [field: SerializeField, Range(1, 10)] public int Index { get; set; } = 1;
        [field: SerializeField] public string Msg { get; set; }
    }
}