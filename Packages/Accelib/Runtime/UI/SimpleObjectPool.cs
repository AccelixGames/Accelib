using System.Collections.Generic;
using UnityEngine;

namespace Accelib.UI
{
    public class SimpleObjectPool : MonoBehaviour
    {
        [SerializeField] private List<GameObject> objects;

        public GameObject GetDisabled() => objects.Find(x => !x.activeSelf) ?? objects[0];
    }
}