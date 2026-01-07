using System.Collections.Generic;
using UnityEngine;

namespace Accelib.Editor.CustomWindow.LevelDesign.Model
{
    [System.Serializable]
    public class LevelDesignDataLayout
    {
        public string label;
        public List<ScriptableObject> dataList;
    }
}