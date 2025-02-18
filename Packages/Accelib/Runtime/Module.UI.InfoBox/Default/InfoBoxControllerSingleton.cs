using System.Collections.Generic;
using Accelib.Core;
using Accelib.Extensions;
using Accelib.Module.UI.InfoBox.Base.Control.Receiver.Interface;
using Accelib.Module.UI.InfoBox.Base.Model;
using NaughtyAttributes;
using UnityEngine;

namespace Accelib.Module.UI.InfoBox
{
    public class InfoBoxControllerSingleton : MonoSingleton<InfoBoxControllerSingleton>
    {
        // [Header("Mouse Tracer")]
        // [SerializeField] private bool enableMouseTracing = false;
        // [SerializeField] private bool enableScreenSafe = false;
        // [SerializeField, ShowIf(nameof(enableMouseTracing))] private RectTransform pivot;
        // [SerializeField, ShowIf(nameof(enableMouseTracing))] private Vector2 offset = Vector2.zero;
        //
        // [Header("Views")]
        // [SerializeField, ReadOnly] private bool isEnabled = false;
        // [SerializeField, ReadOnly] private _IInfoReceiver[] boxViews;
        //
        // [Header("Values")]
        // [SerializeField, ReadOnly] private Vector2 screenHalf;
        //
        // private void Start()
        // {
        //     isEnabled = false;
        //     boxViews = GetComponentsInChildren<_IInfoReceiver>(true);
        //     foreach (var view in boxViews)
        //         view.DrawInfo(null);
        // }
        //
        // private void LateUpdate()
        // {
        //     if (!enableMouseTracing) return;
        //     if (!isEnabled) return;
        //     if (pivot == null) return;
        //     
        //     var mousePos = Input.mousePosition.ToVec2();
        //     screenHalf = new Vector2(Screen.width, Screen.height) * 0.5f;
        //     var pivotPos = Vector2.zero;
        //     var finalOffset = offset;
        //
        //     if (enableScreenSafe)
        //     {
        //         var isRight = mousePos.x >= screenHalf.x;
        //         pivotPos.x = isRight  ? 1f : 0f;
        //         finalOffset.x = offset.x * (isRight ? -1f : 1f);
        //         
        //         var isUp = mousePos.y >= screenHalf.y;
        //         pivotPos.y = isUp ? 1f : 0f;
        //         finalOffset.y = offset.y * (isUp ? 1f : -1f);
        //
        //         pivot.pivot = pivotPos;
        //     }
        //     
        //     //pivot.anchoredPosition = finalOffset + mousePos;
        //     pivot.position = finalOffset + mousePos;
        // }
        //
        // private void Internal_DrawInfoList(List<InfoDataBase> dataList)
        // {
        //     isEnabled = false;
        //     for (var i = 0; i < boxViews.Length; i++)
        //     {
        //         var data = dataList?.GetOrDefault(i);
        //         boxViews[i].DrawInfo(data);
        //         isEnabled = isEnabled || (data != null);
        //     }
        // }
        //
        // private void Internal_DrawInfo(InfoDataBase data)
        // {
        //     isEnabled = data != null;
        //     for (var i = 0; i < boxViews.Length; i++) 
        //         boxViews[i].DrawInfo(i == 0 ? data : null);
        // }

        // public static void DrawInfoList(List<InfoDataBase> dataList) => Instance?.Internal_DrawInfoList(dataList);
        // public static void DrawInfo(InfoDataBase data) => Instance?.Internal_DrawInfo(data);
    }
}