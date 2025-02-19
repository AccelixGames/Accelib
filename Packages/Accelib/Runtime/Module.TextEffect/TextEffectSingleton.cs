using System.Collections.Generic;
using System.Linq;
using Accelib.Core;
using Accelib.Extensions;
using Accelib.Logging;
using Febucci.UI.Core;
using Unity.Collections;
using UnityEngine;

namespace Accelib.Module.TextEffect
{
    public class TextEffectSingleton : MonoSingleton<TextEffectSingleton>
    {
        [SerializeField] private Canvas canvas;
        [SerializeField] private List<TypewriterCore> typewriters = new();

        public static void ShowText(Vector3 worldPos, string text)
        {
            if (Instance == null) return;
            if (string.IsNullOrEmpty(text)) return; 

            Instance.Internal_ShowText(worldPos, text);
        }
        
        private void Internal_ShowText(Vector3 worldPos, string text)
        {
            var writer = typewriters.FirstOrDefault(x => !x.gameObject.activeSelf);
            if (writer == null)
            {
                Deb.LogError("Text pool is full", this);
                return;
            }
            
            // // 월드 좌표 -> 스크린 좌표 변환
            // var cam = Camera.main;
            // if (cam == null) return;
            //
            // var screenPos = cam.WorldToScreenPoint(worldPos).SetZ(0f);
            // Deb.Log($"{worldPos} -> {screenPos}");
            //
            // // 스크린 좌표 -> UI(Canvas) 좌표 변환
            // var canvasRect = writer.transform as RectTransform;
            // if (canvasRect == null) return;
            //
            // // 로컬로 이동
            // RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPos, cam, out var localPoint);
            //
            // // UI 요소 이동
            // canvasRect.anchoredPosition = localPoint;

            writer.gameObject.SetActive(true);
            writer.transform.position = worldPos;
            writer.TextAnimator.SetText(text, true);
            writer.StartShowingText(true);
        }
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Init() => Initialize();
    }
}