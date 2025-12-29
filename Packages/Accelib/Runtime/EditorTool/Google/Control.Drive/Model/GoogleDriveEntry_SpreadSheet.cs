using System.Collections.Generic;

namespace Accelib.EditorTool.Google.Control.Drive.Model
{
    [System.Serializable]
    public class GoogleDriveEntry_SpreadSheet : GoogleDriveEntry
    {
        public override string MimeType => MimeTypeStr.Spreadsheet;
        public override string Url => $"https://docs.google.com/spreadsheets/d/{id}/edit";

        public List<GoogleSheetMetadata> sheets;

        public GoogleDriveEntry_SpreadSheet(string id, string name) : base(id, name)
        {
            sheets = new List<GoogleSheetMetadata>();
        }
    }
}