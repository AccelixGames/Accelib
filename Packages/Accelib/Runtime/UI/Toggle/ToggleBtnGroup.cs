using System;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;

namespace Accelib.UI.Toggle
{
    public class ToggleBtnGroup : MonoBehaviour
    {
        [SerializeField] private List<ToggleBtn> btns;
        [SerializeField, ReadOnly] private int currId = 0;
        
        private void Awake()
        {
            foreach (var btn in btns) 
                btn.Initialized(this);
        }

        private void Start()
        {
            if(btns is not { Count: > 0 }) return;
            
            currId = GetInitialId();
            if(currId < 0 || currId >= btns.Count)
                currId = 0;
            
            for (var i = 0; i < btns.Count; i++) 
                btns[i].Toggle(i == currId, true);
            OnToggleStateChanged(currId);
        }

        internal void Toggle(ToggleBtn btn)
        {
            if(btns is not { Count: > 0 }) return;
            if(btn == null) return;

            var id = btns.FindIndex(x => x == btn);
            if(id < 0) return;

            currId = id;
            for (var i = 0; i < btns.Count; i++) 
                btns[i].Toggle(i == currId);
            OnToggleStateChanged(currId);
        }

        protected virtual int GetInitialId() => 0;

        protected virtual void OnToggleStateChanged(int id) { }

#if UNITY_EDITOR
        private void Reset()
        {
            btns = GetComponentsInChildren<ToggleBtn>().ToList();
        }
#endif
    }
}