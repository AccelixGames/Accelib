using UnityEngine;
using UnityEngine.SceneManagement;

namespace Accelib.Utility
{
    [CreateAssetMenu(fileName = "UtilityTool", menuName = "Accelib/UtilityTool", order = 0)]
    public class SO_UtilityTool : ScriptableObject
    {
        public void OpenURL(string url) => Application.OpenURL(url);

        public void QuitGame()
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