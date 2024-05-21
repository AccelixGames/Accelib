using UnityEngine;
using UnityEngine.SceneManagement;

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
        
        public void LoadScn(int id)
        {
            SceneManager.LoadScene(id);
        }
    }
}