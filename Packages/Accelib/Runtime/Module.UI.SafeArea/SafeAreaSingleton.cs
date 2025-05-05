using System;
using Accelib.AccelixWeb;
using Accelib.Core;
using Accelib.Logging;
using Accelib.Module.UI.SafeArea.Architecture;
using AYellowpaper.SerializedCollections;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

namespace Accelib.Module.UI.SafeArea
{
    public class SafeAreaSingleton : MonoSingleton<SafeAreaSingleton>
    {
        [Flags]
        private enum AreaOption {ApplyX = 1 << 0, ApplyY = 1 << 1}

        [field: Header("현재 엥커")] 
        [field: SerializeField, ReadOnly] public Vector2 CurrAnchorMin { get; private set; }

        [field: SerializeField, ReadOnly]  public Vector2 CurrAnchorMax { get; private set; }

        [Header("이전 값")]
        [SerializeField, ReadOnly] private Rect prevSafeArea = new(0, 0, 0, 0);
        [SerializeField, ReadOnly] private Vector2Int prevScreenSize = new(0, 0);
        [SerializeField, ReadOnly] private ScreenOrientation prevOrientation = ScreenOrientation.AutoRotation;

        [Header("커스텀 값")]
        [SerializeField, ReadOnly] private CustomSafeArea customArea;
        [SerializeField, SerializedDictionary("키", "수치")] private SerializedDictionary<string, CustomSafeArea> customAreas;
  
        [Header("옵션")]
        [SerializeField] private Canvas refCanvas;
        [SerializeField, EnumFlags] private AreaOption safeAreaOption = (AreaOption)int.MaxValue;
        [SerializeField, EnumFlags] private AreaOption customAreaOption = (AreaOption)int.MaxValue;
        [SerializeField] private bool showLog = false;

        [Header("이벤트")]
        public UnityEvent<Vector2, Vector2> onSafeAreaUpdated;

        public void SetCustomArea(string key, CustomSafeArea? customValue)
        {
            if (customValue.HasValue)
            {
                var value = customValue.Value;
                if(!customAreas.TryAdd(key, value))
                    customAreas[key] = value;
            }
            else
                customAreas.Remove(key);
        }
        
        private void OnEnable() => Refresh();

        private void Update() => Refresh();
        
        private Rect GetSafeArea()
        {
#if ACCELIB_AIT
            var insets = AppInTossNative.GetSafeArea();
            var area = new Rect(0f, insets.bottom, Screen.width, Screen.height - insets.top - insets.bottom);
#else
            var area =  Screen.safeArea;
#endif
            if (!safeAreaOption.HasFlag(AreaOption.ApplyX))
            {
                area.x = 0f;
                area.width = Screen.width;
            }
            if (!safeAreaOption.HasFlag(AreaOption.ApplyY))
            {
                area.y = 0f;
                area.height = Screen.height;
            }

            customArea.Reset();
            foreach (var value in customAreas.Values) 
                customArea.Add(value);
            customArea.Multiply(refCanvas.scaleFactor);
            if (customAreaOption.HasFlag(AreaOption.ApplyX))
            {
                area.x += customArea.left;
                area.width -= customArea.left + customArea.right;
            }
            if (customAreaOption.HasFlag(AreaOption.ApplyY))
            {
                area.y += customArea.down;
                area.height -= customArea.down + customArea.up;
            }

            return area;
        }

        private void Refresh()
        {
            // 현재 세이프에리어를 가져옴
            var safeArea = GetSafeArea();

            // 세이프에리어, 스크린사이즈, 회전 모두 변경점이 없다면, 
            if (safeArea == prevSafeArea && 
                Screen.width == prevScreenSize.x && 
                Screen.height == prevScreenSize.y &&
                Screen.orientation == prevOrientation)
                // 종료
                return;
            
            // Fix for having auto-rotate off and manually forcing a screen orientation.
            // See https://forum.unity.com/threads/569236/#post-4473253 and https://forum.unity.com/threads/569236/page-2#post-5166467
            prevScreenSize.x = Screen.width;
            prevScreenSize.y = Screen.height;
            prevOrientation = Screen.orientation;
            prevSafeArea = safeArea;

            ApplySafeArea (safeArea);
        }
        
        private void ApplySafeArea (Rect r)
        { 
            // Check for invalid screen startup state on some Samsung devices (see below)
            if (Screen.width > 0 && Screen.height > 0)
            {
                // Convert safe area rectangle from absolute pixels to normalised anchor coordinates
                var anchorMin = r.position;
                var anchorMax = r.position + r.size;
                anchorMin.x /= Screen.width;
                anchorMin.y /= Screen.height;
                anchorMax.x /= Screen.width;
                anchorMax.y /= Screen.height;

                // Fix for some Samsung devices (e.g. Note 10+, A71, S20) where Refresh gets called twice and the first time returns NaN anchor coordinates
                // See https://forum.unity.com/threads/569236/page-2#post-6199352
                if (anchorMin is { x: >= 0, y: >= 0 } && anchorMax is { x: >= 0, y: >= 0 })
                {
                    CurrAnchorMin = anchorMin;
                    CurrAnchorMax = anchorMax;
                    
                    onSafeAreaUpdated?.Invoke(CurrAnchorMin, CurrAnchorMax);
                    
                    if(showLog)
                        Debug.LogFormat ("New safe area applied to {0}: x={1}, y={2}, w={3}, h={4} on full extents w={5}, h={6}",
                            name, r.x, r.y, r.width, r.height, Screen.width, Screen.height);
                }
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Init() => Initialize();
    }
}