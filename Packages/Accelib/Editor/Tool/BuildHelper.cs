using System.Linq;
using Accelib.Logging;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace Accelib.Editor.Tool
{
    public static class BuildHelper
    {
        public static void AddSceneToBuildSetting(SceneAsset sceneAsset)
        {
            var scns = EditorBuildSettings.scenes;
            for (int i = 0; i < scns.Length; i++)
            {
                var scn = scns[i];
                var scnName = System.IO.Path.GetFileNameWithoutExtension(scn.path);
                
                if(sceneAsset.name == scnName) return;
            }
            
            var settingsScn = new EditorBuildSettingsScene
            {
                path = AssetDatabase.GetAssetPath(sceneAsset),
                enabled = true
            };
            
            Deb.Log(settingsScn.path);
            
            var settingsScns = new EditorBuildSettingsScene[scns.Length + 1];
            for (var i = 0; i < scns.Length; i++) 
                settingsScns[i] = scns[i];
            
            settingsScns[scns.Length] = settingsScn;
            EditorBuildSettings.scenes = settingsScns;
        }
    }
}