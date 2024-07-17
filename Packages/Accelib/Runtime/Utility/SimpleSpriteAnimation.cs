using System;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

namespace Accelib.Utility
{
    public class SimpleSpriteAnimation : MonoBehaviour
    {
        private enum Mode {Image, SpriteRenderer }

        [Header("옵션")]
        [SerializeField] private Mode mode;
        [SerializeField, Range(0.01f, 10f)] private float interval = 0.1f;
        [SerializeField] private Sprite[] sprites;

        [Header("Debug")]
        [SerializeField, ShowIf(nameof(mode), Mode.Image), ReadOnly] private Image image;
        [SerializeField, ShowIf(nameof(mode), Mode.SpriteRenderer), ReadOnly] private SpriteRenderer spRender;
        [SerializeField, ReadOnly] private int _id;
        [SerializeField, ReadOnly] private float _timer;

        private void Awake()
        {
            if(mode == Mode.Image)
                image = GetComponent<Image>();
            if(mode == Mode.SpriteRenderer)
                spRender = GetComponent<SpriteRenderer>();
        }

        private void Start()
        {
            _id = 0;
            _timer = 0f;

            if (mode == Mode.Image && image == null) 
                gameObject.SetActive(false);
            else if (mode == Mode.SpriteRenderer && spRender == null)
                gameObject.SetActive(false);
            else if(sprites is not { Length: > 0 })
                gameObject.SetActive(false);
            else
                SetSprite(sprites[0]);
        }

        private void Update()
        {
            if(sprites is not { Length: > 0 }) return;
            
            _timer += Time.deltaTime;
            if (_timer >= interval)
            {
                _timer -= interval;
                _id = (_id + 1) % sprites.Length;
                SetSprite(sprites[_id]);
            }
        }

        private void SetSprite(Sprite sprite)
        {
            if (mode == Mode.Image)
                image.sprite = sprite;
            else
                spRender.sprite = sprite;
        }

        private void OnValidate() => Awake();
    }
}