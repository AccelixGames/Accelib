using Accelib.Module.Prefs.Data.Base;
using UnityAtoms.BaseAtoms;
using UnityEngine;

namespace Accelib.Module.Prefs.Data
{
    public class PrefsVarString : PrefsVarT<string>
    {
        public PrefsVarString(StringVariable v)
        {
            Variable = v;
            EventBase = v.Changed;
        }

        protected override void Internal_Read() => Variable.Value = PlayerPrefs.GetString(Variable.name, (string)Variable.BaseValue);

        protected override void Internal_Write() => PlayerPrefs.SetString(Variable.name, Variable.Value);
    }
}