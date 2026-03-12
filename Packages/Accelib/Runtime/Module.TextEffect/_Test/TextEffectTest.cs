#if FEBUCCI
using Sirenix.OdinInspector;
using UnityEngine;

namespace Accelib.Module.TextEffect._Test
{
    public class TextEffectTest : MonoBehaviour
    {
        [SerializeField] private string text;

        [Button, EnableIf("@UnityEngine.Application.isPlaying")]
        private void Test() => TextEffectSingleton.ShowText(transform.position, text);
    }
}
#endif