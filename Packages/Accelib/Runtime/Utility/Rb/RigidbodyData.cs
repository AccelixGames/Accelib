using UnityEngine;

namespace Accelix.GameSystem.Utility.Rb
{
    [System.Serializable]
    public class RigidbodyData
    {
        public GameObject target;
        
        public float mass;
        public float linearDamping;
        public float angularDamping;
        public bool automaticCenterOfMass;
        public bool automaticInertiaTensor;
        public bool useGravity;
        public bool isKinematic;
        public RigidbodyInterpolation interpolation;
        public CollisionDetectionMode collisionDetectionMode;
        public RigidbodyConstraints constraints;
        public bool detectCollisions;

        public LayerMask includeLayers;
        public LayerMask excludeLayers;
        
        public Vector3 centerOfMass;
        public Vector3 inertiaTensor;
        public Quaternion inertiaTensorRotation;
    }
}