using Accelib.Audio.Data;
using UnityEditor;
using UnityEngine;

namespace Accelib.Editor.Audio
{
    [CustomEditor(typeof(AudioRefSO))]
    [CanEditMultipleObjects]
    public class AudioRefSO_Editor : UnityEditor.Editor
    {
        public override bool HasPreviewGUI() => ((AudioRefSO)target)?.Clip != null;

        private UnityEditor.Editor _editor;

        public override void OnPreviewSettings()
        {
            var clip = ((AudioRefSO)target)?.Clip;
            if (!clip) return;

            _editor ??= CreateEditor(clip);
            _editor?.OnPreviewSettings();
        }

        public override void OnPreviewGUI(Rect r, GUIStyle background)
        {
            var clip = ((AudioRefSO)target)?.Clip;
            if (!clip) return;
            
            _editor ??= CreateEditor(clip);
            _editor?.OnPreviewGUI(r, background);
        }
    }
}