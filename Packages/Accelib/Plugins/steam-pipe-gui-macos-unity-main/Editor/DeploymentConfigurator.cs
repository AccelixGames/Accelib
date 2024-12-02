using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace Tomicz.Deployer
{
    [CreateAssetMenu(fileName = "DeploymentConfigurator", menuName = "Tomicz/Steam/Depoloyement Target")]
    public class DeploymentConfigurator : ScriptableObject
    {
        public static string SDKPathKey => $"{nameof(DeploymentConfigurator)}_SDKPath";
        
        public BuildTarget BuildTarget => buildTarget;
        public string Description => description + $"(v{Application.version})";
        public string AppName => appName;
        public string SteamUsername => steamUsername;
        public string SDKPath { get; set; }
        public string AppId => appId;
        public string DepotId => depotId;
        public string BranchName => branchName;
        public bool DeleteDoNotShipFolder => deleteDoNotShipFolder;

        [SerializeField] private BuildTarget buildTarget;
        
        [Header("App info")]
        [SerializeField] private string appName = "";
        [SerializeField] private string description = "";
        
        [Header("Steamworks info")]
        [SerializeField] private string steamUsername; 
        [SerializeField] private string appId; 
        [SerializeField] private string depotId; 
        [SerializeField] private string branchName;
        
        [Header("IL2CPP")]
        [Tooltip("A folder YourGameName_BackUpThisFolder_ButDontShipItWithYourGame is auto generated when using IL2CPP scripting backend. Do not upload this folder with your build to Steam. By default it will be deleted. You are given an option to back it up before deleting if you are planning using it for debugging purposes. If you are using Mono backend scripting or making Windows builds, then ignore this field.")]
        [SerializeField] private bool deleteDoNotShipFolder = true;

        public void OnBuildTargetClicked()
        {
            string outputPath = Path.Combine(SDKPath, "tools", "ContentBuilder", "content", $"{buildTarget}", $"{appName}{GetAppExecutionExtension()}");

            BuildPipeline.BuildPlayer(GetScenePaths(), outputPath, buildTarget, BuildOptions.None);

            Debug.Log((int)buildTarget);
            Debug.Log("Target successfully built.");
        }

        private string GetAppExecutionExtension()
        {
            if(buildTarget == BuildTarget.StandaloneOSX)
            {
                return ".app";
            }
            else if(buildTarget == BuildTarget.StandaloneWindows)
            {
                return ".exe";
            }

            return ".exe";
        }

        private static string[] GetScenePaths()
        {
            string[] scenes = new string[EditorBuildSettings.scenes.Length];

            for (int i = 0; i < scenes.Length; i++)
            {
                scenes[i] = EditorBuildSettings.scenes[i].path;
            }

            return scenes;
        }
    }
}