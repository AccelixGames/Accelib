using NaughtyAttributes;
using UnityEngine;

namespace Accelib.Logging
{
    public class TestSentry : MonoBehaviour
    {
        [SerializeField] private string testMessage;
        private void Start()
        {
            SendSentryMessage();
        }

        [Button]
        private void SendSentryMessage()
        {
            SentryLogger.LogWarning("Test", testMessage);
        }
    }
}
