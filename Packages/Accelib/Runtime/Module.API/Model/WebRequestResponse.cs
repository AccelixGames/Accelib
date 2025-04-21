using UnityEngine.Serialization;

namespace Accelib.Module.API.Model
{
    [System.Serializable]
    public class WebRequestResponse<T>
    {
        public bool isSuccess;
        public long status;
        public string message;
        public T data;

        public static WebRequestResponse<T> Exception(string message)
        {
            return new WebRequestResponse<T>
            {
                isSuccess = false,
                status = 500,
                data = default,
                message = message,
            };
        }
    }
}