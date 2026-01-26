using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Accelib.Module.Reference.Control
{
    public class TransformRefFinder : MonoBehaviour
    {
        [Title("# 검색 옵션")]
        [SerializeField] private Transform parent;
        [SerializeField] private string key;

        [Title("# 옵션")] 
        [SerializeField] private bool syncPos = true;
        [SerializeField, ShowIf("syncPos")] private Vector3 posOffset;
        [SerializeField] private bool syncRot = true;
        [SerializeField] private bool syncScl = true;
        
        [Title("# Debug")]
        [ShowInInspector, ReadOnly] private TransformRefProvider _provider;
        
        public Transform Reference => _provider?.transform;

        private void OnEnable()
        {
            parent ??= transform;
            foreach (var provider in parent.GetComponentsInChildren<TransformRefProvider>())
            {
                if (provider.Key != key) continue;
                
                _provider = provider;
                return;
            }
        }

        private void LateUpdate()
        {
            if (!Reference) return;
            
            if(syncPos)
                transform.position = Reference.position + posOffset;
            if(syncRot)
                transform.rotation = Reference.rotation;
            if(syncScl)
                transform.localScale = Reference.localScale;
        }
    }
}