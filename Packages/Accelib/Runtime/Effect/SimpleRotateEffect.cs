using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Accelib.Effect
{
    public class SimpleRotateEffect : MonoBehaviour
    {
        [FormerlySerializedAs("speed")]
        [SerializeField, Range(-1f, 1f)] private float speedX = 1f;
        [SerializeField, Range(-1f, 1f)] private float speedY = 1f;
        [SerializeField, Range(-1f, 1f)] private float speedZ = 1f;
        [SerializeField, Range(0f, 1000f)] private float multiplier = 1f;

        private void Update()
        {
            var delta = multiplier * Time.deltaTime;

            transform.Rotate(
                speedX * delta,
                speedY * delta,
                speedZ * delta);
        }
    }
}