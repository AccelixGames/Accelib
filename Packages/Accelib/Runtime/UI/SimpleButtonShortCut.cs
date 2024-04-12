using System;
using UnityEngine;
using UnityEngine.UI;

namespace Accelib.UI
{
    [RequireComponent(typeof(Button))]
    public class SimpleButtonShortCut : MonoBehaviour
    {
        [SerializeField] private KeyCode shortCutKey;

        private Button _button;

        private void Awake()
        {
            _button = GetComponent<Button>();
        }

        private void Update()
        {
            if(Input.GetKeyDown(shortCutKey))
                _button.onClick.Invoke();
        }
    }
}