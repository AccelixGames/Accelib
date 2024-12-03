using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

namespace Accelib.Editor.Architecture
{
    [System.Serializable]
    public class AppConfig
    {
        [Header("앱 정보")]
        public string name;
        public string appID;
        public string liveBranch;

        [Header("디포 정보")]
        public List<DepotConfig> depots;
    }
}