using System;
using System.Collections.Generic;
using Accelib.Module.AccelNovel.Model;
using Accelix.Accelib.AccelNovel.Model;
using Accelix.Accelib.AccelNovel.Model.Enum;
using UnityEngine;
using UnityEngine.Events;

namespace Accelib.Module.AccelNovel.Control.Action
{
    public abstract class ActionController : MonoBehaviour
    {
        [field: SerializeField] public NovelController Novel { get; private set; }
        
        public abstract string Keyword { get; }
        public abstract bool IsPlaying { get;}
        public virtual bool CanSkip => true;
        protected virtual ENextActionMode DefaultNextActionMode => ENextActionMode.Auto;
        public virtual bool AutoSaveBeforeAction => false;

        public abstract void FromJson(string json);
        public abstract string ToJson();
        
        public virtual void Initialize() {}

        public abstract void PlayAction(ActionLine action, UnityAction<string> playNext);

        public virtual void SkipAction(bool isFastForward = false) {}

        public virtual void ParseResources(ActionLine action, ref HashSet<string> sprites, ref HashSet<string> audios) { }

        public virtual void Release() {}

        protected ENextActionMode ParseNextActionMode(ActionLine action)
        {
            if (action?.arguments?.TryGetValue("next", out var str) ?? false)
            {
                if(Enum.TryParse(str, true, out ENextActionMode mode))
                    return mode;
            }

            return DefaultNextActionMode;
        }
    }
}