using System.Collections.Generic;
using Accelix.GameSystem.Utility.Rb;
using NaughtyAttributes;
using NaughtyWaterBuoyancy.Scripts.Core.Utils;
using UnityEngine;

namespace NaughtyWaterBuoyancy.Scripts.Core
{
    public class FloatingObject : MonoBehaviour
    {
        [Header("# 컴포넌트")]
        [SerializeField] private RigidbodyUtility rbUtility;
        [SerializeField, ReadOnly] private new Rigidbody rigidbody;
        [SerializeField, ReadOnly] private new Collider collider;

        [Header("# 옵션")]        
        // [SerializeField] private bool calculateDensity = false;
        [SerializeField] private float density = 1f;
        [SerializeField] [Range(0f, 1f)] private float normalizedVoxelSize = 0.5f;
        [SerializeField] private float dragInWater = 1f;
        [SerializeField] private float angularDragInWater = 1f;
        
        [Header("# 디버그")]
        [SerializeField, ReadOnly] private WaterVolume water;
        [SerializeField, ReadOnly] private float initialDrag;
        [SerializeField, ReadOnly] private float initialAngularDrag;
        [SerializeField, ReadOnly] private Vector3 voxelSize;
        [SerializeField, ReadOnly] private Vector3[] voxels;

        public Rigidbody Rigidbody => rigidbody;
        public Collider Collider => collider;
        
        private void OnEnable()
        {
            if (!rbUtility) rbUtility = GetComponent<RigidbodyUtility>();
            
            if (rbUtility)
            {
                rigidbody = rbUtility.PrimaryRigidbody;
                collider = rbUtility.PrimaryCollider;
            }
            
            initialDrag = rigidbody.linearDamping;
            initialAngularDrag = rigidbody.angularDamping;
            
            voxels = CutIntoVoxels();

            // if (calculateDensity)
            // {
            //     var objectVolume = MathfUtils.CalculateVolume_Mesh(mesh.mesh, transform);
            //     density = rigidbody.mass / objectVolume;
            // }
        }

        protected virtual void FixedUpdate()
        {
            if (!water || voxels.Length <= 0) return;
            if (!rigidbody) return;
            if (!collider) return;
            
            var forceAtSingleVoxel = CalculateMaxBuoyancyForce() / voxels.Length;
            var bounds = collider.bounds;
            var voxelHeight = bounds.size.y * normalizedVoxelSize;

            var submergedVolume = 0f;
            foreach (var vec3 in voxels)
            {
                var worldPoint = transform.TransformPoint(vec3);
                    
                var waterLevel = water.GetWaterLevel(worldPoint);
                var deepLevel = waterLevel - worldPoint.y + (voxelHeight / 2f); // How deep is the voxel                    
                var submergedFactor = Mathf.Clamp(deepLevel / voxelHeight, 0f, 1f); // 0 - voxel is fully out of the water, 1 - voxel is fully submerged
                submergedVolume += submergedFactor;

                var surfaceNormal = water.GetSurfaceNormal(worldPoint);
                var surfaceRotation = Quaternion.FromToRotation(water.transform.up, surfaceNormal);
                surfaceRotation = Quaternion.Slerp(surfaceRotation, Quaternion.identity, submergedFactor);

                var finalVoxelForce = surfaceRotation * (forceAtSingleVoxel * submergedFactor);
                rigidbody.AddForceAtPosition(finalVoxelForce, worldPoint);

                Debug.DrawLine(worldPoint, worldPoint + finalVoxelForce.normalized, Color.blue);
            }

            submergedVolume /= voxels.Length; // 0 - object is fully out of the water, 1 - object is fully submerged

            rigidbody.linearDamping = Mathf.Lerp(initialDrag, water.Drag * dragInWater, submergedVolume);
            rigidbody.angularDamping = Mathf.Lerp(initialAngularDrag, water.AngularDrag * angularDragInWater, submergedVolume);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!water) other.TryGetComponent(out water);
        }

        private void OnTriggerStay(Collider other)
        {
            if (!water) other.TryGetComponent(out water);
        }

        private void OnTriggerExit(Collider other)
        {
            if (water && other.TryGetComponent<WaterVolume>(out _)) 
                water = null;
        }

        private Vector3 CalculateMaxBuoyancyForce()
        {
            var objectVolume = rigidbody.mass  / density;
            var maxBuoyancyForce = water.Density * objectVolume * -Physics.gravity;

            return maxBuoyancyForce;
        }

        private Vector3[] CutIntoVoxels()
        {
            var initialRotation = transform.rotation;
            transform.rotation = Quaternion.identity;

            var bounds = collider.bounds;
            voxelSize.x = bounds.size.x * normalizedVoxelSize;
            voxelSize.y = bounds.size.y * normalizedVoxelSize;
            voxelSize.z = bounds.size.z * normalizedVoxelSize;
            var voxelsCountForEachAxis = Mathf.RoundToInt(1f / normalizedVoxelSize);
            var voxels = new List<Vector3>(voxelsCountForEachAxis * voxelsCountForEachAxis * voxelsCountForEachAxis);

            for (var i = 0; i < voxelsCountForEachAxis; i++)
            {
                for (var j = 0; j < voxelsCountForEachAxis; j++)
                {
                    for (var k = 0; k < voxelsCountForEachAxis; k++)
                    {
                        var pX = bounds.min.x + voxelSize.x * (0.5f + i);
                        var pY = bounds.min.y + voxelSize.y * (0.5f + j);
                        var pZ = bounds.min.z + voxelSize.z * (0.5f + k);

                        var point = new Vector3(pX, pY, pZ);
                        if (ColliderUtils.IsPointInsideCollider(point, collider, ref bounds))
                        {
                            voxels.Add(transform.InverseTransformPoint(point));
                        }
                    }
                }
            }

            transform.rotation = initialRotation;

            return voxels.ToArray();
        }

#if UNITY_EDITOR
        private void Reset()
        {
            rigidbody = GetComponent<Rigidbody>();
            collider = GetComponentInChildren<Collider>();
            // mesh = collider?.GetComponent<MeshFilter>();
        }
        private void OnDrawGizmosSelected()
        {
            if (voxels != null)
            {
                for (var i = 0; i < voxels.Length; i++)
                {
                    Gizmos.color = Color.magenta - new Color(0f, 0f, 0f, 0.75f);
                    Gizmos.DrawCube(transform.TransformPoint(voxels[i]), voxelSize * 0.8f);
                }
            }
        }
#endif
    }
}
