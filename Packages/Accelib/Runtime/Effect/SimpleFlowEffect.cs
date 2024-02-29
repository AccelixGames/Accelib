using Accelib.Extensions;
using NaughtyAttributes;
using UnityEngine;

namespace Accelib
{
    public class SimpleFlowEffect : MonoBehaviour
    {
        [SerializeField] private Vector3 startPos;
        [SerializeField] private Vector3 endPos;
        [SerializeField, MinMaxSlider(0.01f, 10f)] private Vector2 speedRange;

        [Header("Value")]
        [SerializeField, ReadOnly] private float speed;
        [SerializeField, ReadOnly] private Vector3 vector;

        private void Start()
        {
            speed = speedRange.Random();
            vector = (endPos - startPos).normalized;
        }

        private void Update()
        {
            var spd = speed * Time.deltaTime;
            transform.position += vector * spd;

            if (Vector3.Distance(transform.position, endPos) <= spd)
            {
                transform.position = startPos;
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if(UnityEditor.EditorApplication.isPlaying) return;
            
            Gizmos.DrawSphere(startPos, 0.2f);
            Gizmos.DrawSphere(endPos, 0.2f);
            Gizmos.DrawLine(startPos,endPos);
        }
#endif
    }
}
