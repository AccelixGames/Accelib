#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Accelib.Helper
{
    public class SimpleTextGizmo : MonoBehaviour
    {
        [SerializeField] private string gizmoText = "Hello Gizmo!";
        [SerializeField] private Color gizmoColor = Color.white;
        [SerializeField] private int fontSize = 12;
        [SerializeField] private TextAnchor textAnchor = TextAnchor.MiddleCenter;
        [SerializeField] private  Vector3 textOffset = Vector3.up; // Offset relative to the GameObject's position

        private void OnDrawGizmos()
        {
            // Only draw gizmos when the GameObject is selected
            // You can use OnDrawGizmos() instead if you want it always visible
            OnDrawGizmosSelected(); 
        }

        private void OnDrawGizmosSelected()
        {
            // Ensure we are in the Editor to use Handles
            if (EditorApplication.isPlayingOrWillChangePlaymode) return;

            // Set the position for the label
            var labelPosition = transform.position + textOffset;
            var style = new GUIStyle();
            style.alignment = textAnchor;
            style.normal.textColor = gizmoColor;
            style.fontSize = fontSize;
            
            // Draw the text label
            Handles.Label(labelPosition, gizmoText, style);
        }
    }
}
#endif