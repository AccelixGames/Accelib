using System;
using UnityEngine;

namespace Accelib.Helper
{
    public class SimpleGizmoDrawer : MonoBehaviour
    {
        #if UNITY_EDITOR
        private enum Mode { Always = 0, OnSelected }

        [SerializeField] private Texture2D texture;
        [SerializeField] private Mode mode = Mode.Always; 
        
        private void OnDrawGizmos()
        {
            if(mode == Mode.OnSelected) return;
            
            Draw();
        }

        private void OnDrawGizmosSelected()
        {
            if(mode == Mode.Always) return;

            Draw();
        }

        private void Draw()
        {
            if(texture == null) return;
            Gizmos.DrawIcon(transform.position, texture.name, true);
        }
        #endif
    }
}