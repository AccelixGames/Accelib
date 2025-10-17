using UnityEngine;

namespace Accelix.GameSystem.Utility.Rb
{
    public static class RigidbodyExtension
    {
        // === 저장 ===
        public static RigidbodyData ToData(this Rigidbody rb) => new()
            {
                target = rb.gameObject,
                
                mass = rb.mass,
                linearDamping = rb.linearDamping,
                angularDamping = rb.angularDamping,
                automaticCenterOfMass = rb.automaticCenterOfMass,
                automaticInertiaTensor = rb.automaticInertiaTensor,
                useGravity = rb.useGravity,
                isKinematic = rb.isKinematic,
                interpolation = rb.interpolation,
                collisionDetectionMode = rb.collisionDetectionMode,
                constraints = rb.constraints,
                detectCollisions = rb.detectCollisions,
                
                includeLayers = rb.includeLayers,
                excludeLayers = rb.excludeLayers,

                centerOfMass = rb.centerOfMass,
                inertiaTensor = rb.inertiaTensor,
                inertiaTensorRotation = rb.inertiaTensorRotation
            };

        // === 불러오기 ===
        public static void FromData(this Rigidbody rb, in RigidbodyData data)
        {
            rb.mass = data.mass;
            rb.linearDamping = data.linearDamping;
            rb.angularDamping = data.angularDamping;
            
            if(!data.automaticCenterOfMass)
                rb.centerOfMass = data.centerOfMass;
            rb.automaticCenterOfMass = data.automaticCenterOfMass;

            if (!data.automaticInertiaTensor)
            {
                rb.inertiaTensor = data.inertiaTensor;
                rb.inertiaTensorRotation = data.inertiaTensorRotation;
            }
            rb.automaticInertiaTensor = data.automaticInertiaTensor;
            // rb.ResetInertiaTensor();
            
            rb.useGravity = data.useGravity;
            rb.isKinematic = data.isKinematic;
            rb.interpolation = data.interpolation;
            rb.collisionDetectionMode = data.collisionDetectionMode;
            rb.constraints = data.constraints;
            rb.detectCollisions = data.detectCollisions;

            rb.includeLayers = data.includeLayers;
            rb.excludeLayers = data.excludeLayers;
        }
    }
}