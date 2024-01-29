using System.Reflection;
using UnityEditor;

namespace Accelix.Editor
{
    /// <summary>
    /// 인스펙터 창을 잠금/잠금해제 토글한다.
    /// 단축키: Ctrl+Q
    /// 작성자: 김기민
    /// </summary>
    public static class InspectorLockToggle
    {
        [MenuItem("Tools/Toggle Lock %q")]
        public static void ToggleWindowLock() // Inspector must be inspecting something to be locked
        {
            var windowToBeLocked = EditorWindow.mouseOverWindow; // "EditorWindow.mouseOver Window" can be used instead
 
            if (windowToBeLocked != null  && windowToBeLocked.GetType().Name == "InspectorWindow")
            {
                var type = Assembly.GetAssembly(typeof(UnityEditor.Editor)).GetType("UnityEditor.InspectorWindow");
                var propertyInfo = type.GetProperty("isLocked");
                var value = (bool)propertyInfo.GetValue(windowToBeLocked, null);
                propertyInfo.SetValue(windowToBeLocked, !value, null);
                windowToBeLocked.Repaint();
            }
            else if (windowToBeLocked != null  && windowToBeLocked.GetType().Name == "ProjectBrowser")
            {
                var type = Assembly.GetAssembly(typeof(UnityEditor.Editor)).GetType("UnityEditor.ProjectBrowser");
                var propertyInfo = type.GetProperty("isLocked", BindingFlags.NonPublic | BindingFlags.Public |BindingFlags.Instance);
                
                var value = (bool)propertyInfo.GetValue(windowToBeLocked, null);
                propertyInfo.SetValue(windowToBeLocked, !value, null);
                windowToBeLocked.Repaint();
            }
            else if (windowToBeLocked != null  && windowToBeLocked.GetType().Name == "SceneHierarchyWindow")
            {
                var type = Assembly.GetAssembly(typeof(UnityEditor.Editor)).GetType("UnityEditor.SceneHierarchyWindow");
                
                var fieldInfo = type.GetField("m_SceneHierarchy", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                var propertyInfo = fieldInfo.FieldType.GetProperty("isLocked",
                    BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                var value = fieldInfo.GetValue(windowToBeLocked);
                var value2 = (bool)propertyInfo.GetValue(value);
                propertyInfo.SetValue(value, !value2, null);
                windowToBeLocked.Repaint();
            }
        }
    }
}
