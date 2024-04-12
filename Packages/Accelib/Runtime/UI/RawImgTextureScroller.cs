using System;
using UnityEngine;
using UnityEngine.UI;

namespace Accelib.UI
{
    [RequireComponent(typeof(RawImage))]
    public class RawImgTextureScroller : MonoBehaviour
    {
        [SerializeField] private Vector2 speed;
        
        private RawImage _rawImg;

        private void Awake()
        {
            _rawImg = GetComponent<RawImage>();
        }

        private void Update()
        {
            var uv = _rawImg.uvRect;
            var pos = speed * Time.time;
            pos.x = Mathf.Repeat(pos.x, 1f);
            pos.y = Mathf.Repeat( pos.y, 1f);

            uv.position = pos; 
            _rawImg.uvRect = uv;
        }
    }
}