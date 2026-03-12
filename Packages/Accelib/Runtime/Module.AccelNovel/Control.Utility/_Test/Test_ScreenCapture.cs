using Sirenix.OdinInspector;
using UnityEngine;

namespace Accelib.Module.AccelNovel.Control.Utility._Test
{
    public class Test_ScreenCapture : MonoBehaviour
    {
        [SerializeField] private Texture2D screenShot;

        [Button("Capture"), EnableIf("@UnityEngine.Application.isPlaying")]
        public async void CaptureScreen()
        {
            screenShot = await ScreenCaptureUtility.CaptureScreen();
        }
    }
}