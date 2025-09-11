using System;
using UnityEngine;

namespace Accelib.Helper
{
    public class DontDestroy : MonoBehaviour
    {
        private void Awake()
        {
            DontDestroyOnLoad(this);
        }

        public void Destroy()
        {
            Destroy(this.gameObject);
        }
    }
}