using System.Collections.Generic;

namespace Accelib.Editor.Architecture
{
    /// <summary>단일 빌드의 크기 기록</summary>
    [System.Serializable]
    public class BuildSizeRecord
    {
        public string appId;
        public string buildTarget;
        public string version;
        public string timestamp;
        public long playerBuildSizeBytes;
        public long addressablesSizeBytes;
    }

    /// <summary>빌드 크기 기록 컨테이너 (JSON 직렬화 대상)</summary>
    [System.Serializable]
    public class BuildSizeHistory
    {
        public List<BuildSizeRecord> records = new();
    }
}
