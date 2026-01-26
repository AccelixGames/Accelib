using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;

namespace Accelib.Effect
{
    public class SimpleNavMeshHelper : MonoBehaviour
    {
        [SerializeField] private NavMeshAgent agent;
        [SerializeField] private Transform target;

        [Button]
        public void MoveToTarget()
        {
            agent.SetDestination(target.position);
        }

        private void Reset()
        {
            agent = GetComponent<NavMeshAgent>();
        }
    }
}