using System.Collections.Generic;

namespace Accelib.EditorTool.Google.Model
{
    [System.Serializable]
    public class JSheet
    {
        public string spreadsheetId;
        public List<JSheetData> valueRanges;
    }
}