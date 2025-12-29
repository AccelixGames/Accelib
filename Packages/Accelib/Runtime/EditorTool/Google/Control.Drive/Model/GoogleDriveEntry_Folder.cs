using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace Accelib.EditorTool.Google.Control.Drive.Model
{
    [System.Serializable]
    public class GoogleDriveEntry_Folder : GoogleDriveEntry
    {
        public override string MimeType => MimeTypeStr.Folder;
        public override string Url => $"https://drive.google.com/drive/u/0/folders/{id}";

        [ListDrawerSettings(ShowFoldout = false, ShowItemCount = true, ListElementLabelName = nameof(name))]
        [PolymorphicDrawerSettings(ShowBaseType = false)]
        [OdinSerialize] public List<GoogleDriveEntry> children;

        public GoogleDriveEntry_Folder(string id, string name) : base(id, name)
        {
            children = new List<GoogleDriveEntry>();
        }
    }
}