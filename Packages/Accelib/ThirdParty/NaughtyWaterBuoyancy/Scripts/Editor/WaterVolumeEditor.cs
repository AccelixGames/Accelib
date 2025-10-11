using NaughtyWaterBuoyancy.Scripts.Core;
using UnityEditor;
using UnityEngine;

namespace NaughtyWaterBuoyancy.Editor
{
    [CustomEditor(typeof(WaterVolume))]
    public class WaterVolumeEditor : UnityEditor.Editor
    {
        private const float BOX_COLLIDER_HEIGHT = 5f;

        private WaterVolume waterVolumeTarget;
        private SerializedProperty density;
        private SerializedProperty drag;
        private SerializedProperty angularDrag;
        private SerializedProperty rows;
        private SerializedProperty columns;
        private SerializedProperty quadSegmentSize;
        //private SerializedProperty debugTrans;

        [MenuItem("NaughtyWaterBouyancy/Create Water Mesh")]
        private static void CreateMesh()
        {
            Mesh mesh = WaterMeshGenerator.GenerateMesh(5, 5, 1f);
            AssetDatabase.CreateAsset(mesh, "Assets/NaughtyWaterBuoyancy/Models/Water Mesh.asset");
        }

        protected virtual void OnEnable()
        {
            waterVolumeTarget = (WaterVolume)target;

            density = serializedObject.FindProperty("density");
            drag = serializedObject.FindProperty("drag");
            angularDrag = serializedObject.FindProperty("angularDrag");
            rows = serializedObject.FindProperty("rows");
            columns = serializedObject.FindProperty("columns");
            quadSegmentSize = serializedObject.FindProperty("quadSegmentSize");
            //this.debugTrans = this.serializedObject.FindProperty("debugTrans");

            Undo.undoRedoPerformed += OnUndoRedoPerformed;
        }

        protected virtual void OnDisable()
        {
            Undo.undoRedoPerformed -= OnUndoRedoPerformed;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(density);
            EditorGUILayout.PropertyField(drag);
            EditorGUILayout.PropertyField(angularDrag);

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(rows);
            EditorGUILayout.PropertyField(columns);
            EditorGUILayout.PropertyField(quadSegmentSize);
            if (EditorGUI.EndChangeCheck())
            {
                rows.intValue = Mathf.Max(1, rows.intValue);
                columns.intValue = Mathf.Max(1, columns.intValue);
                quadSegmentSize.floatValue = Mathf.Max(0f, quadSegmentSize.floatValue);

                UpdateMesh(rows.intValue, columns.intValue, quadSegmentSize.floatValue);
                UpdateBoxCollider(rows.intValue, columns.intValue, quadSegmentSize.floatValue);
            }

            //EditorGUILayout.PropertyField(this.debugTrans);

            serializedObject.ApplyModifiedProperties();
        }

        private void UpdateMesh(int rows, int columns, float quadSegmentSize)
        {
            if (Application.isPlaying)
            {
                return;
            }

            MeshFilter meshFilter = waterVolumeTarget.GetComponent<MeshFilter>();
            Mesh oldMesh = meshFilter.sharedMesh;

            Mesh newMesh = WaterMeshGenerator.GenerateMesh(rows, columns, quadSegmentSize);
            newMesh.name = "Water Mesh Instance";

            meshFilter.sharedMesh = newMesh;

            EditorUtility.SetDirty(meshFilter);

            if (oldMesh != null && !AssetDatabase.Contains(oldMesh))
            {
                DestroyImmediate(oldMesh);
            }
        }

        private void UpdateBoxCollider(int rows, int columns, float quadSegmentSize)
        {
            var boxCollider = waterVolumeTarget.GetComponent<BoxCollider>();
            if (boxCollider != null)
            {
                Vector3 size = new Vector3(columns * quadSegmentSize, BOX_COLLIDER_HEIGHT, rows * quadSegmentSize);
                boxCollider.size = size;

                Vector3 center = size / 2f;
                center.y *= -1f;
                boxCollider.center = center;

                EditorUtility.SetDirty(boxCollider);
            }
        }

        private void OnUndoRedoPerformed()
        {
            UpdateMesh(waterVolumeTarget.Rows, waterVolumeTarget.Columns, waterVolumeTarget.QuadSegmentSize);
            UpdateBoxCollider(waterVolumeTarget.Rows, waterVolumeTarget.Columns, waterVolumeTarget.QuadSegmentSize);
        }
    }
}
