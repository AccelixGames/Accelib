using System;
using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Accelib.Utility
{
    public class SceneChanger : MonoBehaviour
    {
        [SerializeField, Scene] private string nextScn;
        [SerializeField, Range(0f, 10f)] private float delay;
        [SerializeField] private bool changeOnlyOneScnActivated = true;

        private void Start()
        {
            if (changeOnlyOneScnActivated)
                if (SceneManager.loadedSceneCount >= 2)
                {
                    SceneManager.UnloadSceneAsync(0);
                    return;
                }

            if (delay > float.MinValue)
                LoadScnDelayed().Forget();
            else
                SceneManager.LoadScene(nextScn, LoadSceneMode.Single);
        }

        private async UniTaskVoid LoadScnDelayed()
        {
            await UniTask.WaitForSeconds(delay);
            
            SceneManager.LoadScene(nextScn, LoadSceneMode.Single);
        }
    }
}