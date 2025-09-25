using UnityEngine;

namespace Accelib
{
    [CreateAssetMenu(fileName = "SO_ChoiceButton", menuName = "Scriptable Objects/SO_ChoiceButton")]
    public class SO_ChoiceButton : ScriptableObject
    {
        [SerializeField, Tooltip("이미 선택 됐던 선택지인지")] public bool chose;
        [field: SerializeField,  TextArea] public string text { get; private set; }
        
    }
}
