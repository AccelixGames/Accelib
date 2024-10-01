using UnityEngine;

namespace Accelib.Editor.GizmoExtension
{
    [RequireComponent(typeof(BoxCollider))]
    public class BoxColliderGizmo : MonoBehaviour
    {
        #if UNITY_EDITOR
        [SerializeField] private BoxCollider boxCollider = null;
        
        private void Reset()
        {
            boxCollider = GetComponent<BoxCollider>();
        }

        private void OnDrawGizmos()
        {
            if (!enabled) return;
        
            Gizmos.color = Color.green;
            
            var size = transform.lossyScale;
            size.x *= boxCollider.size.x;
            size.y *= boxCollider.size.y;
            size.z *= boxCollider.size.z;
            //size = transform.rotation * size;

            Gizmos.DrawWireCube(transform.position + boxCollider.center, size);
        }
        #endif
    }
}