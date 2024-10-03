using System;
using Accelib.Extensions;
using Accelib.Helper;
using NaughtyAttributes;
using Spine.Unity;
using UnityEngine;

#if ACCELIX_SPINE
namespace Accelib.Spine
{
    public class SimpleSpineAnimPlayer_Random : SimpleSpineAnimPlayer
    {
        [Header("Timer")]
        [SerializeField, MinMaxSlider(0f, 10f)] private Vector2 timeRange = Vector2.one;
        [SerializeField] private Timer timer;

        protected override void OnEnable()
        {
            base.OnEnable();
            timer.Set(timeRange.Random());
        }

        private void Update()
        {
            if (timer.OnTime())
            {
                SetAnimation();
                timer.Clear(true);
            }
        }
    }
}
#endif