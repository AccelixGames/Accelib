using Accelib.Module.AccelNovel.Model.Collective.SO.Base;
using UnityEngine;

namespace Accelib.Module.AccelNovel.Model.Collective.SO
{
    [CreateAssetMenu(fileName = "Collective-MiniGame", menuName = "Terri/Collective-MiniGame", order = 4)]
    public class CollectiveSO_MiniGame : CollectiveSO_Base
    {
        [field: SerializeField] public string Title { get; private set; }
    }
}