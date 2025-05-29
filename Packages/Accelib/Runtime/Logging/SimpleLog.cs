using UnityEngine;

namespace Accelib.Logging
{
    public class SimpleLog : MonoBehaviour
    {
        public void Log(string message)
        {
            Debug.Log(message);
        }
    }
}