namespace Accelib.Editor.Architecture
{
    [System.Serializable]
    public class BuildInfo
    {
        // 앱 정보
        public AppConfig app;
        // 디포 정보
        public DepotConfig depot;
        // 버전
        public string versionStr;
        public string versionNumber;
        
        // 경로 정보
        public string buildPath;
        public string logPath;
        public string scriptPath;
    }
}