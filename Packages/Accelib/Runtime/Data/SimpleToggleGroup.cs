using UnityEngine;

namespace Accelib.Data
{
    [System.Serializable]
    public class SimpleToggleGroup
    {
        public GameObject on;
        public GameObject off;

        public void Toggle(bool enable)
        {
            on.SetActive(enable);
            off.SetActive(!enable);
        }
    }
}