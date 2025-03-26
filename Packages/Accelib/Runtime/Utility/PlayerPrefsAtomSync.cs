using System;
using UnityAtoms.BaseAtoms;
using UnityEngine;

namespace Accelib.Utility
{
    public class PlayerPrefsAtomSync : MonoBehaviour
    {
        [SerializeField] private FloatVariable variable;

        private void OnEnable()
        {
            variable.Value = PlayerPrefs.GetFloat(variable.Id);
        }
    }
}