using System;
using UnityAtoms.BaseAtoms;
using UnityEngine;

namespace Accelib.Extension.Atom
{
    public class BoolVariableToggle : MonoBehaviour
    {
        [SerializeField] private BoolVariable variable;
        [SerializeField] private bool reversed = false;
        
        private void OnEnable() => variable.SetValue(!reversed);
        private void OnDisable() => variable.SetValue(reversed);
    }
}