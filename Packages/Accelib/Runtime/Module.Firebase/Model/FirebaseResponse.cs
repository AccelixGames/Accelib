namespace Accelib.Module.Firebase.Model
{
    [System.Serializable]
    public class FirebaseResponse
    {
        public bool success;
        public int status;
        public string msg;

        public static FirebaseResponse Exception(string msg) => new()
        {
            success = false, 
            status = 500, 
            msg = msg
        };

        public override string ToString()
        {
            var res = success ? "Success" : "Failed";
            return $"[{status} {res}] {msg}";
        }
    }
}