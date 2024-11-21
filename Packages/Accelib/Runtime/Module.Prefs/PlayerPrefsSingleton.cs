using System.Collections.Generic;
using Accelib.Core;
using Accelib.Helper;
using Accelib.Logging;
using Accelib.Module.Initialization.Base;
using Accelib.Module.Prefs.Data;
using Accelib.Module.Prefs.Data.Base;
using Accelib.Module.SaveLoad;
using UnityAtoms;
using UnityAtoms.BaseAtoms;
using UnityEngine;

namespace Accelib.Module.Prefs
{
    public class PlayerPrefsSingleton : MonoSingleton<PlayerPrefsSingleton>, ILateInitRequired
    {
        [SerializeField] private List<AtomBaseVariable> variables;
        [SerializeField] private Timer writeTimer;
        
        [Header("Debug")]
        [SerializeReference, SerializeField] private List<PrefsVar> _prefsVars = new();

        public int Priority => 100;

        public void Init()
        {
#if UNITY_SWITCH && !UNITY_EDITOR
            SaveLoadSingleton.ReadPlayerPrefs();
#endif
            
            // Deb.Log("PlayerPrefsSingleton init");
            _prefsVars = new List<PrefsVar>();
            foreach (var variable in variables)
            {
                PrefsVar prefsVar = variable switch
                {
                    IntVariable intVar => new PrefsVarInt(intVar),
                    FloatVariable floatVar => new PrefsVarFloat(floatVar),
                    StringVariable stringVar => new PrefsVarString(stringVar),
                    BoolVariable boolVar => new PrefsVarBool(boolVar),
                    _ => null
                };

                // 추가
                if (prefsVar != null)
                {
                    prefsVar.OnStart();
                    _prefsVars.Add(prefsVar);
                }
                // Null일 경우, 워닝
                else
                    Deb.LogWarning($"Unknown variable type: {variable.name}({variable.GetType()})", variable);
            }
        }

        private void OnDestroy()
        {
            foreach (var prefsVar in _prefsVars) 
                prefsVar.OnDestroy();
        }

        private void LateUpdate()
        {
            if (writeTimer.OnTime())
            {
                var isModified = false;
                foreach (var prefsVar in _prefsVars)
                {
                    var writeResult = prefsVar.Write();
                    isModified = isModified || writeResult; ;
                }

                if (isModified)
                {
                    PlayerPrefs.Save();
#if UNITY_SWITCH && !UNITY_EDITOR
                    SaveLoadSingleton.WritePlayerPrefs();
#endif
                }

                writeTimer.Clear();
            }
        }
    }
}