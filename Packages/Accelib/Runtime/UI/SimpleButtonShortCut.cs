using System;
using UnityEngine;
using UnityEngine.UI;

namespace Accelib.UI
{
    [RequireComponent(typeof(Button))]
    public class SimpleButtonShortCut : MonoBehaviour
    {
        public enum Mode {Always = 0, EnableOnly}
        
        [SerializeField] private KeyCode shortCutKey;
        [SerializeField] private Mode mode = Mode.Always;
        

        private Button _button;

        private void Awake()
        {
            _button = GetComponent<Button>();
        }

        private void Update()
        {
            if(!gameObject.activeSelf)
                if (mode == Mode.EnableOnly) return;
            
            if(Input.GetKeyDown(shortCutKey))
                _button.onClick.Invoke();
        }
    }
}