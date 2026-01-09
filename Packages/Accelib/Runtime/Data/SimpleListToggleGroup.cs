using System.Collections.Generic;
using UnityEngine;

namespace Accelib.Data
{
    [System.Serializable]
    public class SimpleListToggleGroup
    {
        public List<GameObject> on;
        public List<GameObject> off;

        public void Toggle(bool enable)
        {
            foreach (var o in on) 
                o.SetActive(enable);

            foreach (var o in off) 
                o.SetActive(!enable);
        }
    }
}