using Accelib.Extensions;
using NaughtyAttributes;
using UnityEngine;

namespace Accelib.Effect
{
    public class SimpleFlowEffect : MonoBehaviour
    {
        [SerializeField,] private bool useLocalPos = false;
        [SerializeField] private Vector3 startPos;
        [SerializeField] private Vector3 endPos;

        [SerializeField, MinMaxSlider(-10f, 10f)]
        private Vector2 speedRange = new(0.1f, 1f);

        [Header("Value")]
        [SerializeField, ReadOnly] private float speed;
        [SerializeField, ReadOnly] private Vector3 vector;
        [SerializeField, ReadOnly] private Vector3 endWorldPos;

        private void Start()
        {
            speed = speedRange.Random();
            vector = (endPos - startPos).normalized;
            
            endWorldPos = endPos + (useLocalPos ? transform.position : Vector3.zero);
        }

        private void Update()
        {
            var spd = speed * Time.deltaTime;
            
            if(useLocalPos)
                transform.localPosition += vector * spd;
            else
                transform.position += vector * spd;

            var pos = useLocalPos ? transform.localPosition : transform.position;
            
            if (Vector3.Distance(transform.position, endWorldPos) <= Mathf.Abs(spd))
            {
                if (useLocalPos)
                    transform.localPosition = startPos;
                else
                    transform.position = startPos;
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if(UnityEditor.EditorApplication.isPlaying) return;

            var start = startPos + (useLocalPos ? transform.position : Vector3.zero);
            var end = endPos + (useLocalPos ? transform.position : Vector3.zero);
            
            Gizmos.DrawSphere(start, 0.2f);
            Gizmos.DrawSphere(end, 0.2f);
            Gizmos.DrawLine(start,end);
        }
#endif
    }
}
