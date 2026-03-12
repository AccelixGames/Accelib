using Accelib.Module.Prefs.Data.Base;
using UnityAtoms.BaseAtoms;

namespace Accelib.Module.Prefs.Data
{
    [System.Serializable]
    public class PrefsVarBool : PrefsVarT<bool>
    {
        public PrefsVarBool(BoolVariable v)
        {
            Variable = v;
            EventBase = v.Changed;
        }

        protected override void Internal_Read() => ReadJson();

        protected override void Internal_Write() => WriteJson();
    }
}