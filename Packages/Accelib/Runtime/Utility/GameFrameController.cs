using UnityEngine;

namespace Accelib.Utility
{
    public class GameFrameController : MonoBehaviour
    {
        //disable Warning CS0414: The field 'GameFrameController.targetFrameRate' is assigned but its value is never used
        [SerializeField, Range(-1, 165)] private int targetFrameRate = 60;
#if UNITY_EDITOR
        [SerializeField, Range(-1, 165)] private int editorFrameRate = -1;
#endif

        private void Awake()
        {
#if UNITY_EDITOR
            Application.targetFrameRate = editorFrameRate;
#else
            Application.targetFrameRate = targetFrameRate;
#endif
        }
    }
}