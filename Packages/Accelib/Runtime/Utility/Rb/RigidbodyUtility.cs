using Accelib.Extensions;
using NaughtyAttributes;
using UnityEngine;

namespace Accelix.GameSystem.Utility.Rb
{
    public class RigidbodyUtility : MonoBehaviour
    {
        [Header("# 옵션")]
        [SerializeField] private bool alwaysReinitialize = false;
        [SerializeField] private bool autoFindRigidBody = false;
        [SerializeField] private bool autoFindCollider = false;
        
        [Tooltip("AddForce가 적용될 객체 번호")]
        [SerializeField] private int primaryIndex = 0;

        [Header("# 타겟 리지드바디")]
        [SerializeField] private Rigidbody[] rigidbodies;
        [SerializeField] private Collider[] colliders;
        [SerializeField, ReadOnly] private RigidbodyData[] rbDataArray;

        public Rigidbody PrimaryRigidbody => rigidbodies[primaryIndex];
        public Collider PrimaryCollider => colliders[primaryIndex];
        
        private void Awake()
        {
            if(autoFindRigidBody)
                rigidbodies = GetComponentsInChildren<Rigidbody>();
            if(autoFindCollider)
                colliders = GetComponentsInChildren<Collider>();
            
            rbDataArray = new RigidbodyData[rigidbodies.Length];

            for (var i = 0; i < rigidbodies.Length; i++) 
                rbDataArray[i] = rigidbodies[i].ToData();
        }
        
        public void AddForce(Vector3 force, ForceMode impulse)
        {
            var rb = rigidbodies.GetOrDefault(primaryIndex); 
            if (rb != null)
                rb.AddForce(force, impulse);
        }

        public void Toggle(bool enable)
        {
            if (enable)
            {
                for (var i = 0; i < rbDataArray.Length; i++)
                {
                    var target = rbDataArray[i].target;
                    if (!target.TryGetComponent(out rigidbodies[i]))
                        rigidbodies[i] = target.AddComponent<Rigidbody>();
                    
                    rigidbodies[i].FromData(rbDataArray[i]);
                }
            }
            else
            {
                for (var i = 0; i < rigidbodies.Length; i++)
                {
                    // 매번 캐싱 해줘야 한다면,
                    if(alwaysReinitialize)
                        // 저장
                        rbDataArray[i] = rigidbodies[i].ToData();
                    
                    // 리지드바디 삭제
                    Destroy(rigidbodies[i]);
                    rigidbodies[i] = null;
                }
            }
        }

        [Button(enabledMode: EButtonEnableMode.Playmode)] 
        public void SetEnabled() => Toggle(true);
        
        [Button(enabledMode:EButtonEnableMode.Playmode)] 
        public void SetDisabled() => Toggle(false);

#if UNITY_EDITOR
        private void Reset()
        {
            rigidbodies = GetComponentsInChildren<Rigidbody>(true);
        }
#endif
    }
}