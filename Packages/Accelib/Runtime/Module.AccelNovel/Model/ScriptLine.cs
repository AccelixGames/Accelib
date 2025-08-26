using UnityEngine;

namespace Accelib.Module.AccelNovel.Model
{
    [System.Serializable]
    public class ScriptLine
    {
        public string label;
        public string characterKey;
        public string voiceKey;
        [TextArea] public string text;
    }
}