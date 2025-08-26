using Accelib.Module.AccelNovel.Model.Collective.SO.Base;
using UnityEngine;

namespace Accelib.Module.AccelNovel.Model.Collective.SO
{
    [CreateAssetMenu(fileName = "Collective-SoundTrack", menuName = "Terri/Collective-SoundTrack", order = 1)]
    public class CollectiveSO_SoundTrack : CollectiveSO_Base
    {
        [field: SerializeField] public string Title { get; set; }
        [field: SerializeField, TextArea] public string Hint { get; set; }
        [field: SerializeField] public string[] ThumbnailKeys { get; set; }
    }
}