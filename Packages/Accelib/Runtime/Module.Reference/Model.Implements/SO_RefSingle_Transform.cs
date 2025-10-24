using UnityEngine;

namespace Accelib.Module.Reference.Model.Implements
{
    [CreateAssetMenu(fileName = FileNamePrefix+nameof(Transform), menuName = MenuNamePrefix + nameof(Transform), order = Order - 1)]
    public class SO_RefSingle_Transform : SO_RefSingle<Transform> { }
}