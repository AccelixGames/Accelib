using System;
using Accelib.Module.AccelNovel.Model;
using Accelix.Accelib.AccelNovel.Model;
using AYellowpaper.SerializedCollections;
using UnityEngine;
using UnityEngine.Events;

namespace Accelib.Module.AccelNovel.Control.Action.Effect.Base
{
    public abstract class ActionController_Effect : ActionControllerT<ActionController_Effect.Data>
    {
        [Serializable]
        public class Data
        {
            public string key;
        }
        
        public override bool IsPlaying => false;

        [Header("Values")]
        [SerializeField] private SerializedDictionary<string, GameObject> effects;
        [SerializeField] private Data currData; 
        
        protected override void Internal_FromJson(Data data)
        { 
            currData = data;
            Toggle();
        }

        protected override Data Internal_GetData() => currData;
        
        public override void Initialize()
        {
            currData = new Data();
            
            foreach (var (_, eff) in effects) 
                eff.SetActive(false);
        }

        public override void PlayAction(ActionLine action, UnityAction<string> playNext)
        {
            currData.key = action.value;
            Toggle();
            
            playNext?.Invoke(null);
        }

        private void Toggle()
        {
            // 모든 이펙트 비활성화
            foreach (var (_, obj) in effects)
                obj.SetActive(false);

            // key가 유효하고, 해당 key에 해당하는 이펙트가 있다면 활성화
            if (!string.IsNullOrEmpty(currData.key) && effects.TryGetValue(currData.key, out var particle))
            {
                particle.SetActive(true);
            }
        }
    }
}