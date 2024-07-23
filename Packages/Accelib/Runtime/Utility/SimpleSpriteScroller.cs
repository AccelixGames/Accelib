using System;
using UnityEngine;

namespace Accelib.Utility
{
    [RequireComponent(typeof(Renderer))]
    public class SimpleSpriteScroller : MonoBehaviour
    {
        private enum RenderMode {Instance = 0, Shared}
        private enum UpdateMode { Update = 0, LateUpdate, FixedUpdate }

        [Header("오프셋")]
        [SerializeField] private Vector2 offset;
        
        [Header("옵션")]
        [SerializeField] private UpdateMode updateMode = UpdateMode.Update;
        [SerializeField] private RenderMode renderMode = RenderMode.Instance;
        
        private Renderer _render;
        private Material _mat;
        private static readonly int MainTex = Shader.PropertyToID("_MainTex");

        private void Awake()
        {
            _render = GetComponent<Renderer>();
            _mat = _render.material;
        }

        private void Update()
        {
            if(updateMode == UpdateMode.Update) UpdateOffset();
        }

        private void LateUpdate()
        {
            if(updateMode == UpdateMode.LateUpdate) UpdateOffset();
        }

        private void FixedUpdate()
        {
            if(updateMode == UpdateMode.FixedUpdate) UpdateOffset();
        }

        private void UpdateOffset()
        {
            offset.x = Mathf.Repeat(offset.x, 1f);
            offset.y = Mathf.Repeat(offset.y, 1f);
            
            if(renderMode == RenderMode.Instance)
                _mat.SetTextureOffset(MainTex, offset);
            else
                _render.sharedMaterial.SetTextureOffset(MainTex, offset);
        }
    }
}
