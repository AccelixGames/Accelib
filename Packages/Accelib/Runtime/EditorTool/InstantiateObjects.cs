using System;
using NaughtyAttributes;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
namespace Accelib.EditorTool
{
    public class InstantiateObjects : MonoBehaviour
    {
        [SerializeField] private GameObject prefab;
        [SerializeField] private Vector2Int count;
        [SerializeField] private Vector2 interval;
        
        [Header("Name")]
        [SerializeField] private string naming;
        [SerializeField, ReadOnly] private string namingPreview;

        [Button(enabledMode: EButtonEnableMode.Editor)]
        private void Instantiate()
        {
            for (var y = 0; y < count.y; y++)
            for (var x = 0; x < count.x; x++)
            {
                var n = naming.Replace("{x}", x.ToString()).Replace("{y}", y.ToString());
                var posX = x * interval.x;
                var posY = y * interval.y;

                var inst = PrefabUtility.InstantiatePrefab(prefab, transform) as GameObject;
                inst.name = n;
                inst.transform.position = new Vector3(posX, posY, 0f);
            }
        }

        private void OnValidate()
        {
            namingPreview = naming?.Replace("{x}", "3")?.Replace("{y}", "3");
        }
    }
}
#endif