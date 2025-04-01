#if FEBUCCI
using NaughtyAttributes;
using UnityEngine;

namespace Accelib.Module.TextEffect._Test
{
    public class TextEffectTest : MonoBehaviour
    {
        [SerializeField] private string text;

        [Button(enabledMode: EButtonEnableMode.Playmode)]
        private void Test() => TextEffectSingleton.ShowText(transform.position, text);
    }
}
#endif