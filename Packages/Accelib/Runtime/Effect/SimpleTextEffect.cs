using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

namespace Accelib.Effect
{
    public class SimpleTextEffect : MonoBehaviour
    {
        [SerializeField] private List<string> textList;
        [SerializeField, Range(0.01f, 10f)] private float interval = 0.1f;

        private TMP_Text text; 
        private StringBuilder builder;
        
        private int id;
        private float timer;

        private void Awake()
        {
            text = GetComponent<TMP_Text>();
            builder = new StringBuilder(textList[id]);
            
            if (text == null || textList.Count <= 0) 
                enabled = false;
        }

        private void OnEnable()
        {
            if (!enabled) return;
            
            timer = 0f;
            id = 0;
            text.SetText(builder);
        }

        private void Update()
        {
            timer += Time.unscaledDeltaTime;
            if (timer >= interval)
            {
                timer -= interval;
                id = (id + 1) % textList.Count;

                builder.Clear();
                builder.Append(textList[id]);
                text.SetText(builder);
            }
        }
    }
}