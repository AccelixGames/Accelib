using Accelib.Module.AccelNovel.Model.Collective.SO.Base;
using UnityEngine;

namespace Accelib.Module.AccelNovel.Model.Collective.Utility
{
    public class CollectiveUtility : MonoBehaviour
    {
        [SerializeField] private CollectiveSO_Base target;
        [SerializeField] private bool collectOnEnable = false;

        private void OnEnable()
        {
            if(collectOnEnable) TryCollect();
        }

        public void TryCollect() => target.OnCollect();
    }
}