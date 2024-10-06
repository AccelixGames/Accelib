using System.Linq;
using Accelib.Logging;
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
    public static class SceneHelper
    {
        private const int InitScnBuildID = 0;
        
        private static string _prevActiveScnPath;
        private static string[] _prevLoadedScnPaths;

        public static string[] EnabledEditorScnPaths => 
            (from scene in EditorBuildSettings.scenes where scene.enabled select scene.path)
            .ToArray();
        
        [MenuItem("Accelix/PlayInitializeScn %h")]
        public static void PlayInitializeScn()
        {
            var initScn = EnabledEditorScnPaths[InitScnBuildID];
            PlayScnWithHistory(initScn);
        }
        
        public static void PlayScnWithHistory(string path)
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
                var currActiveScn = SceneManager.GetActiveScene();
                var scnCount = SceneManager.sceneCount;
                
                // 액티브 씬 저장
                _prevActiveScnPath = currActiveScn.path;
                
                // 로드된 씬 저장
                _prevLoadedScnPaths = new string[scnCount];
                for (var i = 0; i < scnCount; i++) 
                    _prevLoadedScnPaths[i] = SceneManager.GetSceneAt(i).path;
                
                // 저장
                if (currActiveScn.isDirty)
                    EditorSceneManager.SaveModifiedScenesIfUserWantsTo(new[] {currActiveScn});

                // 씬 전환 이벤트 등록(플레이 모드 종료 후 다시 원래 씬으로 돌아오도록)
                EditorApplication.playModeStateChanged += ReturnToPreviousScn;
                    
                // Init 씬 열기
                EditorSceneManager.OpenScene(path);
               
                // PlayMode 전환
                EditorApplication.EnterPlaymode();
            }
        }

        private static void ReturnToPreviousScn(PlayModeStateChange state)
        {
            // Edit 모드로 들어올 때만 작동
            if (state != PlayModeStateChange.EnteredEditMode) return;
            
            // 이전 씬을 모두 순회하며,
            for (var i = 0; i < _prevLoadedScnPaths.Length; i++)
            {
                // Path
                var prevLoadedScnPath = _prevLoadedScnPaths[i];
                
                // 패쓰가 비었다면 넘기기
                if (string.IsNullOrEmpty(prevLoadedScnPath)) continue;

                // 씬 열기
                var scn = EditorSceneManager.OpenScene(prevLoadedScnPath, i == 0 ? OpenSceneMode.Single : OpenSceneMode.Additive);

                // 액티브 씬이였다면,
                if (prevLoadedScnPath == _prevActiveScnPath)
                    // 액티브 씬으로 만들기
                    SceneManager.SetActiveScene(scn);
            }

            // 이벤트 빼기
            EditorApplication.playModeStateChanged -= ReturnToPreviousScn;
        }
    }
}
