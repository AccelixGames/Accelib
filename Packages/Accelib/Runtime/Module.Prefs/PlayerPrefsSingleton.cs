using System;
using System.Collections.Generic;
using System.Linq;
using Accelib.Core;
using Accelib.Helper;
using Accelib.Logging;
using Accelib.Module.Prefs.Data;
using Accelib.Module.Prefs.Data.Base;
using NaughtyAttributes;
using UnityAtoms;
using UnityAtoms.BaseAtoms;
using UnityEngine;

namespace Accelib.Module.Prefs
{
    public class PlayerPrefsSingleton : MonoSingleton<PlayerPrefsSingleton>
    {
        [SerializeField] private List<AtomBaseVariable> variables;
        [SerializeField] private Timer writeTimer;
        
        [SerializeReference, SerializeField] private List<PrefsVar> _prefsVars = new();

        protected override void Awake()
        {
            base.Awake();
            
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
                    isModified = isModified || prefsVar.Write();
                
                if (isModified) PlayerPrefs.Save();

                writeTimer.Clear(true);
            }
        }
    }
}