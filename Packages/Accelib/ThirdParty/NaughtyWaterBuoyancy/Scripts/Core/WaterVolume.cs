using System.Collections.Generic;
using NaughtyAttributes;
using NaughtyWaterBuoyancy.Scripts.Core.Collections;
using UnityEngine;

namespace NaughtyWaterBuoyancy.Scripts.Core
{
    [RequireComponent(typeof(BoxCollider))]
    [RequireComponent(typeof(MeshFilter))]
    public class WaterVolume : MonoBehaviour
    {
        // public const string TAG = "Water Volume";

        [Header("# 물 옵션")]
        [SerializeField] private float density = 1f;
        [SerializeField] private float drag = 3f;
        [SerializeField] private float angularDrag = 2f;

        [Header("# 매쉬 옵션")]
        [SerializeField] private int rows = 10;
        [SerializeField] private int columns = 10;
        [SerializeField] private float quadSegmentSize = 1f;

        [Header("# 디버그")]
        [SerializeField, ReadOnly] private Mesh mesh;
        [SerializeField, ReadOnly] private Vector3[] meshLocalVertices;
        [SerializeField, ReadOnly] private Vector3[] meshWorldVertices;

        public float Density => density;
        public float Drag => drag;
        public float AngularDrag => angularDrag;
        
        public int Rows => rows;
        public int Columns => columns;
        public float QuadSegmentSize => quadSegmentSize;

        private Mesh Mesh
        {
            get
            {
                if (!mesh) mesh = GetComponent<MeshFilter>().mesh;
                return mesh;
            }
        }

        protected virtual void Awake()
        {
            CacheMeshVertices();
        }

        protected virtual void Update()
        {
            CacheMeshVertices();
        }

        protected virtual void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.matrix = transform.localToWorldMatrix;

            Gizmos.DrawWireCube(GetComponent<BoxCollider>().center, GetComponent<BoxCollider>().size);
        }

        protected virtual void OnDrawGizmos()
        {
            if (!Application.isPlaying)
            {
                Gizmos.color = Color.cyan - new Color(0f, 0f, 0f, 0.75f);
                Gizmos.matrix = transform.localToWorldMatrix;

                Gizmos.DrawCube(GetComponent<BoxCollider>().center - Vector3.up * 0.01f, GetComponent<BoxCollider>().size);

                Gizmos.color = Color.cyan - new Color(0f, 0f, 0f, 0.5f);
                Gizmos.DrawWireCube(GetComponent<BoxCollider>().center, GetComponent<BoxCollider>().size);

                Gizmos.matrix = Matrix4x4.identity;
            }
            else
            {
                // Draw sufrace normal
                //var vertices = this.meshWorldVertices;
                //var triangles = this.Mesh.triangles;
                //for (int i = 0; i < triangles.Length; i += 3)
                //{
                //    Gizmos.color = Color.white;
                //    Gizmos.DrawLine(vertices[triangles[i + 0]], vertices[triangles[i + 1]]);
                //    Gizmos.DrawLine(vertices[triangles[i + 1]], vertices[triangles[i + 2]]);
                //    Gizmos.DrawLine(vertices[triangles[i + 2]], vertices[triangles[i + 0]]);

                //    Vector3 center = MathfUtils.GetAveratePoint(vertices[triangles[i + 0]], vertices[triangles[i + 1]], vertices[triangles[i + 2]]);
                //    Vector3 normal = this.GetSurfaceNormal(center);

                //    Gizmos.color = Color.green;
                //    Gizmos.DrawLine(center, center + normal);
                //}

                // Draw mesh vertices
                //if (this.meshWorldVertices != null)
                //{
                //    for (int i = 0; i < this.meshWorldVertices.Length; i++)
                //    {
                //        DebugUtils.DrawPoint(this.meshWorldVertices[i], Color.red);
                //    }
                //}

                // Test GetSurroundingTrianglePolygon(Vector3 worldPoint);
                //if (debugTrans != null)
                //{
                //    Gizmos.color = Color.blue;
                //    Gizmos.DrawSphere(debugTrans.position, 0.1f);

                //    var point = debugTrans.position;
                //    var triangle = this.GetSurroundingTrianglePolygon(point);
                //    if (triangle != null)
                //    {
                //        Gizmos.color = Color.red;

                //        Gizmos.DrawLine(triangle[0], triangle[1]);
                //        Gizmos.DrawLine(triangle[1], triangle[2]);
                //        Gizmos.DrawLine(triangle[2], triangle[0]);
                //    }
                //}
            }
        }

        public Vector3[] GetSurroundingTrianglePolygon(Vector3 worldPoint)
        {
            var localPoint = transform.InverseTransformPoint(worldPoint);
            var x = Mathf.CeilToInt(localPoint.x / QuadSegmentSize);
            var z = Mathf.CeilToInt(localPoint.z / QuadSegmentSize);
            if (x <= 0 || z <= 0 || x >= (Columns + 1) || z >= (Rows + 1))
                return null;
            if (meshWorldVertices.Length <= 0)
                return null;
            
            var trianglePolygon = new Vector3[3];
            var indexZX = GetIndex(z, x);
            var indexZ1X1 = GetIndex(z - 1, x - 1);
            var indexZ1X = GetIndex(z-1, x);
            var indexZX1 = GetIndex(z, x - 1);
            Debug.Log($"Len {meshWorldVertices.Length} / {indexZX}, {indexZ1X1}, {indexZ1X}, {indexZX1}");
            
            if ((worldPoint - meshWorldVertices[indexZX]).sqrMagnitude < (worldPoint - meshWorldVertices[indexZ1X1]).sqrMagnitude)
            {
                trianglePolygon[0] = meshWorldVertices[indexZX];
            }
            else
            {
                trianglePolygon[0] = meshWorldVertices[indexZ1X1];
            }

            trianglePolygon[1] = meshWorldVertices[indexZ1X];
            trianglePolygon[2] = meshWorldVertices[indexZX1];

            return trianglePolygon;
        }

        public Vector3[] GetClosestPointsOnWaterSurface(Vector3 worldPoint, int pointsCount)
        {
            MinHeap<Vector3> allPoints = new MinHeap<Vector3>(new Vector3HorizontalDistanceComparer(worldPoint));
            for (int i = 0; i < meshWorldVertices.Length; i++)
            {
                allPoints.Add(meshWorldVertices[i]);
            }

            Vector3[] closestPoints = new Vector3[pointsCount];
            for (int i = 0; i < closestPoints.Length; i++)
            {
                closestPoints[i] = allPoints.Remove();
            }

            return closestPoints;
        }

        public Vector3 GetSurfaceNormal(Vector3 worldPoint)
        {
            Vector3[] meshPolygon = GetSurroundingTrianglePolygon(worldPoint);
            if (meshPolygon != null)
            {
                Vector3 planeV1 = meshPolygon[1] - meshPolygon[0];
                Vector3 planeV2 = meshPolygon[2] - meshPolygon[0];
                Vector3 planeNormal = Vector3.Cross(planeV1, planeV2).normalized;
                if (planeNormal.y < 0f)
                {
                    planeNormal *= -1f;
                }

                return planeNormal;
            }

            return transform.up;
        }

        public float GetWaterLevel(Vector3 worldPoint)
        {
            var meshPolygon = GetSurroundingTrianglePolygon(worldPoint);
            if (meshPolygon is { Length: > 2 })
            {
                var planeV1 = meshPolygon[1] - meshPolygon[0];
                var planeV2 = meshPolygon[2] - meshPolygon[0];
                var planeNormal = Vector3.Cross(planeV1, planeV2).normalized;
                if (planeNormal.y < 0f)
                {
                    planeNormal *= -1f;
                }

                // Plane equation
                var yOnWaterSurface = (-(worldPoint.x * planeNormal.x) - (worldPoint.z * planeNormal.z) + Vector3.Dot(meshPolygon[0], planeNormal)) / planeNormal.y;
                //Vector3 pointOnWaterSurface = new Vector3(point.x, yOnWaterSurface, point.z);
                //DebugUtils.DrawPoint(pointOnWaterSurface, Color.magenta);

                return yOnWaterSurface;
            }

            return transform.position.y;
        }

        public bool IsPointUnderWater(Vector3 worldPoint)
        {
            return GetWaterLevel(worldPoint) - worldPoint.y > 0f;
        }

        private int GetIndex(int row, int column)
        {
            return Mathf.Clamp(row * (Columns + 1) + column, 0, meshWorldVertices.Length - 1);
        }

        private void CacheMeshVertices()
        {
            meshLocalVertices = Mesh.vertices;
            meshWorldVertices = ConvertPointsToWorldSpace(meshLocalVertices);
        }

        private Vector3[] ConvertPointsToWorldSpace(Vector3[] localPoints)
        {
            Vector3[] worldPoints = new Vector3[localPoints.Length];
            for (int i = 0; i < localPoints.Length; i++)
            {
                worldPoints[i] = transform.TransformPoint(localPoints[i]);
            }

            return worldPoints;
        }

        private class Vector3HorizontalDistanceComparer : IComparer<Vector3>
        {
            private Vector3 distanceToVector;

            public Vector3HorizontalDistanceComparer(Vector3 distanceTo)
            {
                distanceToVector = distanceTo;
            }

            public int Compare(Vector3 v1, Vector3 v2)
            {
                v1.y = 0;
                v2.y = 0;
                float v1Distance = (v1 - distanceToVector).sqrMagnitude;
                float v2Distance = (v2 - distanceToVector).sqrMagnitude;

                if (v1Distance < v2Distance)
                {
                    return -1;
                }
                else if (v1Distance > v2Distance)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
        }
    }
}
