using UnityEngine;

namespace Accelib.Utility
{
    [CreateAssetMenu(fileName = "ApplicationHandler", menuName = "Accelib/ApplicationHandler", order = 0)]
    public class ApplicationHandler : ScriptableObject
    {
        public void Quit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}