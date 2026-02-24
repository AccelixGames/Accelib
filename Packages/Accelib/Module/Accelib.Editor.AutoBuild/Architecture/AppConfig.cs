using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Accelib.Editor.Architecture
{
    [System.Serializable]
    public class AppConfig
    {
        [TitleGroup("앱 정보")]
        public string name;
        [TitleGroup("앱 정보")]
        public string appID;
        [TitleGroup("앱 정보")]
        public string liveBranch;

        [TitleGroup("디포 정보")]
        public List<DepotConfig> depots;
    }
}
