using System.Collections.Generic;
using Accelib.Data;
using UnityEngine;

namespace Accelib.Editor.CustomWindow.LevelDesign.Model
{
    [CreateAssetMenu(fileName = "FILENAME", menuName = "MENUNAME", order = 0)]
    public class LevelDesignConfig : ScriptableObjectCached<LevelDesignConfig>
    {
        [SerializeField] private List<LevelDesignDataLayout> menuLayouts;
        public IReadOnlyList<LevelDesignDataLayout> MenuLayouts => menuLayouts;
    }
}