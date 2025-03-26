using System;
using UnityEngine;

namespace Accelib.Module.ScreenCapture
{
    public class ScreenCaptureHandler : MonoBehaviour
    {
        [Header("Capture")]
        [SerializeField] private Camera captureCam;
        [SerializeField] private RenderTexture camRenderTex;
        [SerializeField] private TextureFormat targetTexFormat = TextureFormat.ARGB32;
        
        public Texture2D CaptureRT()
        {
            try
            {
                // 캡쳐 카메라 활성화
                captureCam.gameObject.SetActive(true);
                
                // RenderTexture를 활성화
                RenderTexture.active = camRenderTex;

                // RenderTexture 크기에 맞는 Texture2D 생성
                var outputTex = new Texture2D(camRenderTex.width, camRenderTex.height, targetTexFormat, false);

                // RenderTexture의 내용을 Texture2D로 읽기
                outputTex.ReadPixels(new Rect(0, 0, camRenderTex.width, camRenderTex.height), 0, 0);
                outputTex.Apply();
                
                // 리턴
                return outputTex;
            }
            catch (Exception e)
            {
                Debug.LogException(e, this);
                return null;
            }
            finally
            {
                // RenderTexture 비활성화
                RenderTexture.active = null;
                
                // 캡쳐 카메라 비활성화
                captureCam.gameObject.SetActive(false);
            }
        }
    }
}