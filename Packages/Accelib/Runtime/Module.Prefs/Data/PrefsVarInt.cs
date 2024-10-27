using Accelib.Module.Prefs.Data.Base;
using UnityAtoms.BaseAtoms;
using UnityEngine;

namespace Accelib.Module.Prefs.Data
{
    [System.Serializable]
    public class PrefsVarInt : PrefsVarT<int>
    {
        public PrefsVarInt(IntVariable v)
        {
            Variable = v;
            EventBase = v.Changed;
        }

        protected override void Internal_Read() => Variable.Value = PlayerPrefs.GetInt(Variable.name, (int)Variable.BaseValue);

        protected override void Internal_Write() => PlayerPrefs.SetInt(Variable.name, Variable.Value);
    }
}