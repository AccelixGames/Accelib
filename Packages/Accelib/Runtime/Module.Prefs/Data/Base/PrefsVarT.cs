using System;
using Accelib.Logging;
using Newtonsoft.Json;
using UnityAtoms;
using UnityEngine;

namespace Accelib.Module.Prefs.Data.Base
{
    [System.Serializable]
    public abstract class PrefsVarT<T> : PrefsVar
    {
        protected AtomBaseVariable<T> Variable;

        protected void ReadJson()
        {
            try
            {
                var json = PlayerPrefs.GetString(Variable.name, null);
                if (json == null) 
                    Variable.Value = (T)Variable.BaseValue;
                else 
                    Variable.Value = JsonConvert.DeserializeObject<T>(json);
            }
            catch (Exception e)
            {
                Deb.LogException(e, Variable);
            }
        }
        
        protected void WriteJson()
        {
            try
            {
                var json = JsonConvert.SerializeObject(Variable.Value);
                PlayerPrefs.SetString(Variable.name, json);
            }
            catch (Exception e)
            {
                Deb.LogException(e, Variable);
            }
        }
    }
}