using System;
using System.Collections.Generic;
using System.Linq;
using Accelib.Core;
using Accelib.Logging;
using Accelib.Module.UI.Popup.Data;
using Accelib.Module.UI.Popup.Layer;
using Accelib.Module.UI.Popup.Layer.Base;
using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using UnityAtoms.BaseAtoms;
using UnityEngine;
using UnityEngine.UI;

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

        public bool IsModalActive => modalPopup.gameObject.activeSelf;
        public int LayerCount => layerPopups.Count;
        
        private void Start()
        {
            isPaused.SetValue(false);
            canvas.gameObject.SetActive(false);
            modalPopup.gameObject.SetActive(false);
        }
        
        #region **** Open ****
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
            dim.transform.SetAsLastSibling();
            
            // 레이어 생성 및 열기
            var layer = Instantiate(prefab, canvas.transform);
            layerPopups.Add(layer);
            layer.OpenLayer(param);

            isPaused.SetValue(true);
            return true;
        }
        
        /// <summary>
        /// 모달 팝업을 연다.
        /// </summary>
        public async UniTask<LayerPopup_Modal.Result> OpenModal(ModalOpenOption option)
        {
            if (modalPopup.gameObject.activeSelf)
            {
                Deb.LogError("이미 모달이 열린 상태입니다.");
                return LayerPopup_Modal.Result.Exception;
            }
         
            canvas.gameObject.SetActive(true);
            dim.SetAsLastSibling();
            modalPopup.transform.SetAsLastSibling();
            
            return await modalPopup.Open(option);
        }
        #endregion
        
        #region **** Close ***
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

            // 후처리
            OnLayerPopupClosed();
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
        
        // 모달을 닫는다.
        internal void CloseModal()
        {
            modalPopup.gameObject.SetActive(false);
            OnLayerPopupClosed();
        }

        // 레이어 팝업 닫힌 후 후처리
        private void OnLayerPopupClosed()
        {
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
        }
        #endregion
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Init() => Initialize();
    }
}