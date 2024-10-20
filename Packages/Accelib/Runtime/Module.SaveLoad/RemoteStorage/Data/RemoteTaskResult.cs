namespace Accelib.Module.SaveLoad.RemoteStorage.Data
{
    [System.Serializable]
    public class RemoteTaskResult
    {
        public string message;
        public bool success;
        public byte[] data;
        
        public RemoteTaskResult(bool success, byte[] data, string msg = "")
        {
            this.success = success;
            this.data = data;
            
            if(string.IsNullOrEmpty(msg))
                message = success ? "success" : "failed";
            else
                message = msg;
        }

        public static RemoteTaskResult Failed(string msg) => new(false, null, msg);
    }
}