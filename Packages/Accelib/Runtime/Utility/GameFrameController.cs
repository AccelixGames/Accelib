#pragma warning disable CS0414

using Sirenix.OdinInspector;
using UnityEngine;

namespace Accelib.Utility
{
    public class GameFrameController : MonoBehaviour
    {
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

#if UNITY_EDITOR
        [Button]
        private void Set()
        {
            Application.targetFrameRate = editorFrameRate;
        }
#endif
    }
}