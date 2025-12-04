#if UNITY_EDITOR
using UnityEngine;

namespace Accelib.GizmoExtension
{
    [RequireComponent(typeof(BoxCollider))]
    public class BoxColliderGizmo : MonoBehaviour
    {
        [SerializeField] private BoxCollider boxCollider = null;
        [SerializeField] private Color color =  Color.white;
        
        private void Reset()
        {
            boxCollider = GetComponent<BoxCollider>();
        }

        private void OnDrawGizmos()
        {
            if (!enabled) return;
        
            Gizmos.color = color;
            
            var size = transform.lossyScale;
            size.x *= boxCollider.size.x;
            size.y *= boxCollider.size.y;
            size.z *= boxCollider.size.z;
            //size = transform.rotation * size;

            Gizmos.DrawWireCube(transform.position + boxCollider.center, size);
        }
    }
}
#endif