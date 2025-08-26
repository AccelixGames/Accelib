using Accelib.Module.AccelNovel.Model.Collective.SO.Base;
using UnityEngine;

namespace Accelib.Module.AccelNovel.Model.Collective.SO
{
    [CreateAssetMenu(fileName = "Collective-Picture", menuName = "Terri/Collective-Picture", order = 0)]
    public class CollectiveSO_Picture : CollectiveSO_Base
    {
        [field: SerializeField] public string Title { get; set; }
        [field: SerializeField, TextArea] public string Description { get; set; }
        [field: SerializeField, TextArea] public string Hint { get; set; }
    }
}