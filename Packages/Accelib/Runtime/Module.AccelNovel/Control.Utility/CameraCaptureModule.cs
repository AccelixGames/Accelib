using Accelib.Core;
using NaughtyAttributes;
using UnityEngine;

namespace Accelib.Module.AccelNovel.Control.Utility
{
    public class CameraCaptureModule : MonoSingleton<CameraCaptureModule>
    {
        [Header("Camera Settings")]
        [SerializeField] private Camera baseCam;
        //[SerializeField] private Camera uiCam;
        [SerializeField] private RenderTexture captureRT;
        [SerializeField] private TextureFormat textureFormat = TextureFormat.RGBA32;
        [SerializeField] private bool linear;
        
        // [Header("")]
        // [SerializeField, ShowAssetPreview] private Texture2D output;

        public static Texture2D Capture() => Instance?.Internal_Capture();

        [Button]
        private Texture2D Internal_Capture()
        {
            return null;

            // if (baseCam == null || captureRT == null)
            // {
            //     Debug.LogError("카메라 또는 RT가 설정되지 않았습니다.", this);
            //     return null;
            // }
            //
            // // 2. 원래 카메라 설정 백업
            // var baseRT = baseCam.targetTexture;
            // var baseData = baseCam.GetUniversalAdditionalCameraData();
            // var baseMask = baseData.volumeLayerMask;
            //
            // // 3. 카메라에 RenderTexture 적용하고 렌더링
            // baseData.volumeLayerMask = 0;
            // baseCam.targetTexture = captureRT;
            //
            // // 4. 렌더
            // baseCam.Render();
            //
            // // 4. Texture2D로 읽어오기
            // RenderTexture.active = captureRT;
            // var capturedTexture = new Texture2D(captureRT.width, captureRT.height, textureFormat, false, linear);
            // capturedTexture.ReadPixels(new Rect(0, 0, captureRT.width, captureRT.height), 0, 0);
            // capturedTexture.Apply();
            //
            // // 5. 설정 복구
            // baseData.volumeLayerMask = baseMask;
            // baseCam.targetTexture = baseRT;
            // RenderTexture.active = null;
            //
            // // 캡쳐된 이미지 리턴
            // return capturedTexture;
        }
    }
}