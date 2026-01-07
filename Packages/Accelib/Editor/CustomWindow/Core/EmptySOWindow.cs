using Accelib.Editor.Extension;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace Accelib.Editor.CustomWindow.Core
{
    [System.Serializable]
    public class EmptySOWindow<T> where T : ScriptableObject
    {
        [ShowInInspector] private string Msg => $"{typeof(T).Name} 을 찾을 수 없습니다.";

        [SerializeField, LabelText("에셋 이름")] private string assetName;
        [SerializeField, FolderPath(RequireExistingPath = true), LabelText("생성 경로")] private string assetPath;
        [Button("생성하기")]
        private void Create()
        {
            if (string.IsNullOrEmpty(assetName))
            {
                EditorUtility.DisplayDialog("오류", "올바르지 않은 이름입니다.: \n" + assetName, "닫기");
                return;
            }

            if (!AssetDatabase.IsValidFolder(assetPath))
            {
                EditorUtility.DisplayDialog("오류", "올바르지 않은 경로입니다: \n" + assetPath, "닫기");
                return;
            }
            
            var asset = SOUtility.CreateAsset<T>(assetName, assetPath);
            EditorGUIUtility.PingObject(asset);
        }

        public EmptySOWindow()
        {
            assetName = typeof(T).Name;
            assetPath = "Assets/Accelix/Config";
        }
    }
}