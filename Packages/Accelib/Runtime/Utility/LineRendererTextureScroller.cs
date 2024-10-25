using System;
using UnityEngine;

namespace Accelib.Utility
{
    [RequireComponent(typeof(LineRenderer))]
    public class LineRendererTextureScroller : MonoBehaviour
    {
        private static readonly int MainTex = Shader.PropertyToID("_MainTex");
        private LineRenderer _line;

        [SerializeField] private Vector2 spd;
        [SerializeField, Range(-10f, 10f)] private float multiplier = 1f;

        private void Awake()
        {
            _line = GetComponent<LineRenderer>();
            if (_line == null) enabled = false;
        }

        private void Update()
        {
            _line.material.SetTextureOffset(MainTex, spd * (multiplier * Time.time));   
        }
    }
}