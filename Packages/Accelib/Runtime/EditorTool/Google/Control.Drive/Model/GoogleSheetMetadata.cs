namespace Accelib.EditorTool.Google.Control.Drive.Model
{
    [System.Serializable]
    public class GoogleSheetMetadata
    {
        public string sheetId;
        public string gid;
        public string name;
        
        public string Url => $"https://docs.google.com/spreadsheets/d/{sheetId}/edit?gid={gid}";

        public GoogleSheetMetadata(string sheetId, string gid, string name)
        {
            this.sheetId = sheetId;
            this.gid = gid;
            this.name = name;
        }
    }
}