using System;
using UnityEngine;

namespace Accelib.Effect
{
    public class SimpleRotateEffect : MonoBehaviour
    {
        [SerializeField] private float speed = 1f;

        private void Update()
        {
            transform.Rotate(0, 0, speed*Time.deltaTime);
        }
    }
}