using System;
using System.Collections.Generic;
using UnityAtoms;
using UnityEngine;

namespace Accelib.Extension.Atom
{
    public class EventResetter : MonoBehaviour
    {
        [SerializeField] private List<AtomEventBase> events;

        private void OnEnable()
        {
            for (var i = 0; i < events.Count; i++)
            {
                events[i].UnregisterAll();
            }
        }
    }
}