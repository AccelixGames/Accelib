using System;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Accelib.Utility
{
    public class SimpleRandomSprite : MonoBehaviour
    {
        public enum EnableMode { Awake, Start, OnEnable }
        
        [SerializeField] private EnableMode enableMode = EnableMode.Awake;
        [SerializeField] private Sprite[] sprites;
        
        private Image _image;
        private SpriteRenderer _spriteRenderer;

        private void Awake()
        {
            _image = GetComponent<Image>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
            
            UpdateSprite(EnableMode.Awake);
        }
        private void Start() => UpdateSprite(EnableMode.Start);
        private void OnEnable() => UpdateSprite(EnableMode.OnEnable);

        private void UpdateSprite(EnableMode mode)
        {
            if (enableMode != mode)
                return;
            if (sprites.Length <= 0)
                return;
            
            var sprite = sprites[Random.Range(0, sprites.Length)];

            if (_spriteRenderer != null)
                _spriteRenderer.sprite = sprite;
            if(_image != null)
                _image.sprite = sprite;
        }
    }
}
