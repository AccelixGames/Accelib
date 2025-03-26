using System;
using Accelib.Module.Audio;
using Accelib.Module.Audio.Data;
using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Accelib.Module.Transition.Utility
{
    [CreateAssetMenu(fileName = "SceneChanger", menuName = "Accelib/SceneChanger", order = 10)]
    public class TransitionSceneChanger : ScriptableObject
    {
        private enum SceneType
        {
            Name = 0,
            Index
        }

        [Header("LoadScn")]
        [SerializeField]  private SceneType sceneType = SceneType.Name;
        [ShowIf(nameof(sceneType), SceneType.Name), SerializeField, Scene] private string targetScene;
        [ShowIf(nameof(sceneType), SceneType.Index), SerializeField, Scene] private int targetIndex;
        
        [Header("LoadMode")]
        [SerializeField] private LoadSceneMode loadMode = LoadSceneMode.Single;
        [SerializeField, Range(0, 128)] private int frameDelay = 0;
        [SerializeField] private bool fadeOutBgm = false;

        [Header("Current")]
        [SerializeField, ReadOnly] private bool isLocked = false;

        private void OnEnable() => isLocked = false;
        private void OnDisable() => isLocked = false;

        [Button(enabledMode: EButtonEnableMode.Playmode)]
        public void Change() => ChangeAsync().Forget();

        public async UniTask ChangeAsync()
        {
            // 잠금 체크 및 설정
            if(isLocked) return;
            isLocked = true;
            
            // 프레임 딜레이
            if(frameDelay > 0)
                await UniTask.DelayFrame(frameDelay);
            
            // BGM
            if(fadeOutBgm)
                AudioSingleton.StopChannel(AudioChannel.Bgm, true);

            // 트렌지션 시작
            await TransitionSingleton.StartTransition();
            
            // 씬 전환
            if(sceneType == SceneType.Name)
                await SceneManager.LoadSceneAsync(targetScene, loadMode);
            else
                await SceneManager.LoadSceneAsync(targetIndex, loadMode);
            
            // 트렌지션 종료
            await TransitionSingleton.EndTransition();
            
            // 잠금 해제
            isLocked = false;
        }
    }
}