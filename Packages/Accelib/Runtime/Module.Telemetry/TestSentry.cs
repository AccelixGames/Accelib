using NaughtyAttributes;
using UnityEngine;

namespace Accelib.Module.Telemetry
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
            SentryLogger.LogDebug("Test", testMessage);
        }

    }
}
