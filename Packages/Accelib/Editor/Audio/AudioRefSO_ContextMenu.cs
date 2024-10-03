using System.IO;
using Accelib.Audio.Data;
using UnityEditor;
using UnityEngine;

namespace Accelib.Editor.Audio
{
    public class AudioRefSO_ContextMenu : UnityEditor.Editor
    {
        private const string Path = "Assets/Accelib/AudioRef 생성";
        
        // 폴더를 우클릭했을 때 나오는 메뉴 항목 추가
        [MenuItem(Path, true)]
        private static bool CreateAudioRefs_Validate()
        {
            // 선택한 객체가 폴더인지 확인
            return Selection.activeObject != null && 
                   AssetDatabase.IsValidFolder(AssetDatabase.GetAssetPath(Selection.activeObject));
        }

        [MenuItem(Path)]
        private static void CreateAudioRefs()
        {
            // 선택한 폴더 경로 가져오기
            var folderPath = AssetDatabase.GetAssetPath(Selection.activeObject);

            // 폴더 안의 모든 파일 가져오기
            var fileEntries = Directory.GetFiles(folderPath, "*", SearchOption.AllDirectories);
        
            // AudioClip들의 이름을 출력
            foreach (var filePath in fileEntries)
            {
                // 파일이 AudioClip인지 확인
                var audioClip = AssetDatabase.LoadAssetAtPath<AudioClip>(filePath);
                if (audioClip != null) 
                    AudioRefSO.CreateAssetFromClip(audioClip, folderPath, false);
            }
            
            // 저장
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();   
        }
    }
}