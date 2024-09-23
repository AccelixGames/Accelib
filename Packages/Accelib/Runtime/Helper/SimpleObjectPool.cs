using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Accelib.Helper
{
    public class SimpleObjectPool : MonoBehaviour
    {
        [SerializeField] private List<GameObject> objects;

        private void Reset()
        {
            objects = new List<GameObject>();
            foreach (Transform t in transform) objects.Add(t.gameObject);
        }

        public GameObject GetDisabled() => objects.Find(x => !x.activeSelf) ?? objects[0];
    }
}