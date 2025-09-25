using UnityEngine;

namespace Accelib.Module.AccelTag
{
    [CreateAssetMenu( fileName = "(AccelTag)", menuName = "Accelib/New Tag", order = 0 )]
    public class SO_AccelTag : ScriptableObject
    {
        [SerializeField, TextArea] private string comment;
    }
}