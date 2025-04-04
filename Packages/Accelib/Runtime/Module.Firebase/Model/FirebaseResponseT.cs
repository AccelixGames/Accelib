using Newtonsoft.Json;

namespace Accelib.Module.Firebase.Model
{
    [System.Serializable]
    public class FirebaseResponseT<T> : FirebaseResponse where T : class
    {
        public T body;
        
        public new static FirebaseResponseT<T> Exception(string msg) => new()
        {
            success = false, 
            status = 500, 
            msg = msg,
            body = null
        };
        
        public override string ToString()
        {
            var res = success ? "Success" : "Failed";
            return $"[{status} {res}] {msg}\n" +
                   $"{JsonConvert.SerializeObject(body)}";
        }
    }
}