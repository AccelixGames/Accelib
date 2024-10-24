using System;
using System.Collections.Generic;
using System.Linq;
using Accelib.Core;
using Accelib.Logging;
using Accelib.Module.UI.Popup.Layer;
using Accelib.Module.UI.Popup.Layer.Base;
using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using UnityAtoms.BaseAtoms;
using UnityEngine;

namespace Accelib.Module.UI.Popup
{
    public class PopupSingleton : MonoSingleton<PopupSingleton>
    {
        [Header("Var")]
        [SerializeField] private BoolVariable isPaused;

        [Header("Base")]
        [SerializeField] private Canvas canvas;
        [SerializeField] private Transform dim;
        
        [Header("Current")]
        [SerializeField] private LayerPopup_Modal modalPopup;
        [SerializeField, ReadOnly] private List<LayerPopupBase> layerPopups;

        private void Start()
        {
            isPaused.SetValue(false);
            canvas.gameObject.SetActive(false);
            modalPopup.gameObject.SetActive(false);
        }

        /// <summary>
        /// 모달 팝업을 연다.
        /// </summary>
        public async UniTask<LayerPopup_Modal.Result> OpenModal(string title, string desc, string ok, string ng, bool useLocale)
        {
            if (modalPopup.gameObject.activeSelf)
            {
                Deb.LogError("이미 모달이 열린 상태입니다.");
                return LayerPopup_Modal.Result.Exception;
            }
         
            canvas.gameObject.SetActive(true);
            dim.SetAsLastSibling();
            
            return await modalPopup.Open(title, desc, ok, ng, useLocale);
        }

        /// <summary>
        /// 레이어 팝업을 연다.
        /// </summary>
        public bool OpenLayer(LayerPopupBase prefab, object param = null)
        {
            // 만약 동일한 창을 띄울 수 없다면,
            if (!prefab.AllowMultiInstance)
                // 타입을 비교해서, 동일한 타입이 있다면,
                if (layerPopups.Any(popup => popup.GetType() == prefab.GetType()))
                {
                    Deb.LogWarning($"해당 팝업은 중복해서 열 수 없습니다: {prefab.name}", prefab);
                    // 실패
                    return false;
                }
            
            // 이전 팝업을 포커스 잃기
            if (layerPopups.Count > 0)
            {
                var lastPopup = layerPopups[^1];
                lastPopup.OnLostFocus();
                if (lastPopup.HideOnLostFocus) 
                    lastPopup.gameObject.SetActive(false);
            }
            
            // 딤 위치 옮기기
            canvas.gameObject.SetActive(true);
            dim.SetAsLastSibling();
            
            // 레이어 생성 및 열기
            var layer = Instantiate(prefab, canvas.transform);
            layerPopups.Add(layer);
            layer.OpenLayer(param);

            isPaused.SetValue(true);
            return true;
        }
        
        /// <summary>
        /// 레이어 팝업을 닫는다.
        /// </summary>
        public bool CloseLayer(LayerPopupBase target)
        {
            // 레이어 찾기
            var layer = layerPopups.Find(x => x == target);
            if (layer == null) return false;

            // 레이어 지우기
            layerPopups.Remove(layer);
            layer.OnClose();
            Destroy(layer.gameObject);
            
            // 열린게 아직 있다면,
            if (layerPopups.Count > 0)
            {
                // 마지막 팝업 포커스 되찾기
                var lastPopup = layerPopups[^1];
                lastPopup.gameObject.SetActive(true);
                lastPopup.OnRegainFocus();
                
                // 딤 위치 변경
                dim.SetSiblingIndex(layerPopups.Count - 1);
            }
            // 다 닫혔다면,
            else
            {
                // 캔버스 닫기
                canvas.gameObject.SetActive(false);
                // 일시정지 해제
                isPaused.SetValue(false);
            }

            return true;
        }
        
        /// <summary>
        /// 마지막 레이어 팝업을 닫는다.
        /// </summary>
        [Button(enabledMode: EButtonEnableMode.Playmode)]
        public bool CloseLastLayer()
        {
            if (layerPopups.Count > 0)
                return CloseLayer(layerPopups[^1]);
            
            return false;
        }
        
        // private void LateUpdate()
        // {
        //     var layerEnabled = layerPopup?.IsEnabled() ?? false; 
        //     var modalEnabled = modalPopup?.IsEnabled() ?? false;
        //     currIsOpen = layerEnabled || modalEnabled;
        //     
        //     // 모달이 켜져있으면, 레이어 팝업 보이기/숨기기 처리 
        //     if (layerEnabled) 
        //         layerPopup.Show(!modalEnabled);
        //     
        //     if(currIsOpen == prevIsOpen) return;
        //     prevIsOpen = currIsOpen;
        //     
        //     // 모달 또는 레이어가 활성화 된 상태면, 일시정지
        //     if (isPaused.Value != currIsOpen) 
        //         isPaused.SetValue(currIsOpen);
        // }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Init() => Initialize();
    }
}