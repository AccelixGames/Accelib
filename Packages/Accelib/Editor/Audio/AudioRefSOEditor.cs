using Accelib.Audio.Data;
using Accelib.Logging;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Accelib.Editor.Audio
{
    [CustomEditor(typeof(AudioRefSO_Default))]
    [CanEditMultipleObjects]
    public class AudioRefSOEditor : UnityEditor.Editor
    {
        public override bool HasPreviewGUI() => ((AudioRefSO_Default)target)?.Clip != null;

        private UnityEditor.Editor _editor;

        public override void OnPreviewSettings()
        {
            var clip = ((AudioRefSO_Default)target)?.Clip;
            if (!clip) return;

            _editor ??= CreateEditor(clip);
            _editor?.OnPreviewSettings();
        }

        public override void OnPreviewGUI(Rect r, GUIStyle background)
        {
            var clip = ((AudioRefSO_Default)target)?.Clip;
            if (!clip) return;
            
            _editor ??= CreateEditor(clip);
            _editor?.OnPreviewGUI(r, background);
        }
    }
}