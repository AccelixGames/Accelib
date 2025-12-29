using UnityEngine;

namespace Accelib.EditorTool.Google.Control.Drive.Model
{
    [System.Serializable]
    public abstract class GoogleDriveEntry
    {
        public abstract string MimeType { get; }
        public abstract string Url { get; }
        
        public string id;
        public string name;

        public GoogleDriveEntry(string id = null, string name = null)
        {
            this.id = id;
            this.name = name;
        }

        public void OpenURL()
        {
            Application.OpenURL(Url);
        }
    }
}