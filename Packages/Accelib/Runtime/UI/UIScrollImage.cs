using System;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

namespace Accelib.UI
{
    [RequireComponent(typeof(Image))]
    public class UIScrollImage : MonoBehaviour
    {
        [SerializeField] private Vector2 speed;
        [SerializeField, ReadOnly] private Vector2 offset;
        
        private Image _img;

        private void Awake() => _img = GetComponent<Image>();

        private void Start()
        {
            _img.material = new Material(_img.material);
        }

        private void Update()
        {
            offset.x = Mathf.Repeat(Time.time * speed.x, 1);
            offset.y = Mathf.Repeat(Time.time * speed.y, 1);

            _img.material.mainTextureOffset = offset;
        }
    }
}