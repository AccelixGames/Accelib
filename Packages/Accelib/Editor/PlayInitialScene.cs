using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Accelib.Editor
{
    /// <summary>
    /// 게임을 Initialize 씬부터 플레이한다.
    /// 작성자: 김기민
    /// </summary>
    public static class PlayInitialScene
    {
        private const int InitScnBuildID = 0;
        
        private static string _previousScene;
        
        [MenuItem("Accelix/PlayInitializeScn %h")]
        public static void PlayInitializeScn()
        {   
            // 이미 플레이중이라면,
            if (Application.isPlaying)
            {
                // 플레이 종료
                EditorApplication.ExitPlaymode();
            }
            // 플레이중이 아니라면,
            else
            {
                // 현재 씬 가져오기
                var currScn = EditorSceneManager.GetActiveScene();
                
                // 저장
                if (currScn.isDirty)
                    EditorSceneManager.SaveModifiedScenesIfUserWantsTo(new[] {currScn});

                // 현재 씬이 Init 씬이 아니라면,
                if (currScn.buildIndex != InitScnBuildID)
                {
                    // 현재 씬 정보 캐싱
                    _previousScene = currScn.path;

                    // 씬 전환 이벤트 등록(플레이 모드 종료 후 다시 원래 씬으로 돌아오도록)
                    EditorApplication.playModeStateChanged += ReturnToPreviousScn;
                    
                    // Init 씬 열기
                    var initScn = EnabledEditorScenes[InitScnBuildID];
                    EditorSceneManager.OpenScene(initScn);
                }
               
                // PlayMode 전환
                EditorApplication.EnterPlaymode();
            }
        }

        private static void ReturnToPreviousScn(PlayModeStateChange state)
        {
            // Edit 모드로 들어올 때만 작동
            if (state != PlayModeStateChange.EnteredEditMode) return;
            
            // 이전 씬이 비어있지 않다면,
            if (!string.IsNullOrEmpty(_previousScene))
                // 이전 씬 오픈
                EditorSceneManager.OpenScene(_previousScene);

            // 이벤트 빼기
            EditorApplication.playModeStateChanged -= ReturnToPreviousScn;
        }
        
        private static string[] EnabledEditorScenes => 
            (from scene in EditorBuildSettings.scenes 
                where scene.enabled 
                select scene.path).ToArray();
    }
}
