using System;
using Accelib.Logging;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Accelib.Module.AccelNovel.Control.Utility
{
    public static class ScreenCaptureUtility
    {
        public static async UniTask<Texture2D> CaptureScreen(int size = 512)
        {
            await UniTask.WaitForEndOfFrame();

            Texture2D tex = null;
            Texture2D squareTex = null;
            RenderTexture rt = null;

            try
            {
                // Step 1: 화면 캡처
                tex = UnityEngine.ScreenCapture.CaptureScreenshotAsTexture();

                // Step 2: 정사각형 자르기 (짧은 변 기준)
                var squareSize = Mathf.Min(tex.width, tex.height);
                var startX = (int)((tex.width - squareSize) * 0.5f);
                var startY = (int)((tex.height - squareSize) * 0.5f);

                squareTex = new Texture2D(squareSize, squareSize, TextureFormat.RGBA32, false);
                squareTex.SetPixels(tex.GetPixels(startX, startY, squareSize, squareSize));
                squareTex.Apply();

                // Step 3: GPU 리사이징 (Blit + RenderTexture)
                rt = RenderTexture.GetTemporary(size, size, 0, RenderTextureFormat.ARGB32);
                RenderTexture.active = rt;
                Graphics.Blit(squareTex, rt);

                // Step 4: GPU -> 텍스처 복사
                var resizedTex = new Texture2D(size, size, TextureFormat.RGBA32, false);
                resizedTex.ReadPixels(new Rect(0, 0, size, size), 0, 0);
                resizedTex.Apply();

                // 결과물 반환
                return resizedTex;
            }
            catch (Exception e)
            {
                Deb.LogException(e);
                return null;
            }
            finally
            {
                // Step 5: 메모리 정리
                if (tex) Object.Destroy(tex);
                if (squareTex) Object.Destroy(squareTex);
                if (rt != null)
                {
                    RenderTexture.active = null;
                    RenderTexture.ReleaseTemporary(rt);
                }
            }
        }
    }
}