using Accelib.Module.Audio;
using Accelib.Module.Audio.Data._Base;
using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using UnityEngine;

namespace Accelib.Module.AccelNovel.Control.Action.Function
{
    public class GameEndingScreen : MonoBehaviour
    {
        [Header("")]
        [SerializeField, Range(0f, 10f)] private float delayTime = 3f;
        [SerializeField, Scene] private string nextScn;
        
        [Header("Audio")]
        [SerializeField] private AudioRefBase[] audioRefs;
        
        public async UniTaskVoid OpenEndingScreen()
        {
            if(gameObject.activeSelf) return;
            
            AudioSingleton.StopAllChannel(true);
            
            gameObject.SetActive(true);
            await UniTask.WaitForSeconds(delayTime);
            
            // LoadingSystem.ChangeScene(nextScn);
        }
    }
}