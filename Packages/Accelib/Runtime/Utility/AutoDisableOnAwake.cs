using System;
using UnityEngine;

namespace Accelib.Utility
{
    public class AutoDisableOnAwake : MonoBehaviour
    {
        private void Awake()
        {
            gameObject.SetActive(false);
        }
    }
}