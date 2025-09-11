using System.Collections.Generic;

namespace Accelib.EditorTool.Google.Model
{
    [System.Serializable]
    public class JSheetData
    {
        public string range;
        public string majorDimension;
        public List<List<string>> values;
    }
}