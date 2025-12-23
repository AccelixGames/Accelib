using System.Collections.Generic;

namespace Accelib.EditorTool.Google.Model.Metadata
{
    [System.Serializable]
    public class JSheetMetadata
    {
        public string spreadsheetId;
        public string spreadsheetUrl;
        public Property properties;
        public List<Sheet> sheets;

        [System.Serializable]
        public class Property
        {
            public string title;
            public string locale;
            public string autoRecalc;
            public string timeZone;
        }
        
        [System.Serializable]
        public class Sheet
        {
            public Property properties;
            public object conditionalFormats;
            
            [System.Serializable]
            public class Property
            {
                public int sheetId;
                public string title;
                public int index;
                public string sheetType;
            }
        }
    }
}