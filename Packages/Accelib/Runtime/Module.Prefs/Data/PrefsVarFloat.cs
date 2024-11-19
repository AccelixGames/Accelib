using Accelib.Module.Prefs.Data.Base;
using UnityAtoms.BaseAtoms;
using UnityEngine;

namespace Accelib.Module.Prefs.Data
{
    [System.Serializable]
    public class PrefsVarFloat : PrefsVarT<float>
    {
        public PrefsVarFloat(FloatVariable v)
        {
            Variable = v;
            EventBase = v.Changed;
        }

        protected override void Internal_Read()
        {
            Variable.Value = PlayerPrefs.GetFloat(Variable.name, (float)Variable.BaseValue);
        }

        protected override void Internal_Write() => PlayerPrefs.SetFloat(Variable.name, Variable.Value);
    }
}