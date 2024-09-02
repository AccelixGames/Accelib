using System;
using Accelib.Helper;
using NaughtyAttributes;
using UnityEngine;

namespace Accelib.Effect
{
    public class SimpleStepRotateEffect : MonoBehaviour
    {
        [SerializeField] private Vector3 rotationA;
        [SerializeField] private Vector3 rotationB;
        [SerializeField, Range(0f, 60f)] private float interval = 1f;

        [Header("")]
        [SerializeField, ReadOnly] private bool rotSwitch = false;
        [SerializeField] private Timer timer;

        private void Start()
        {
            timer.Set(interval);
            transform.rotation = Quaternion.Euler(rotationA);
            rotSwitch = false;
        }

        private void Update()
        {
            if (timer.OnTime())
            {
                transform.rotation = Quaternion.Euler(rotSwitch ? rotationA : rotationB);

                rotSwitch = !rotSwitch;
                timer.Set(interval);
            }
        }
    }
}