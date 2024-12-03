using UnityEditor;
using UnityEditor.Build.Profile;

namespace Accelib.Editor.Architecture
{
    [System.Serializable]
    public class DepotConfig
    {
        public string depotID;
        public BuildTarget buildTarget;
        public BuildProfile buildProfile;
        public bool includeInBuild = true;
    }
}