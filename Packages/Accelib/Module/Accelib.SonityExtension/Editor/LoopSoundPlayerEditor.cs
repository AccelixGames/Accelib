using Accelib.SonityExtension.Runtime;
using Sirenix.OdinInspector.Editor;
using Sonity.Internal;
using UnityEditor;
using UnityEngine;

namespace Accelib.SonityExtension.Editor
{
    /// <summary>
    /// LoopSoundPlayer 커스텀 에디터 (OdinEditor 기반).
    /// Odin 어트리뷰트를 유지하면서 SoundContainer Quick Setup GUI를 추가한다.
    /// </summary>
    [CustomEditor(typeof(LoopSoundPlayer))]
    public class LoopSoundPlayerEditor : OdinEditor
    {
        private bool _showSetup = true;
        private float _pitchLowSemitone;
        private float _pitchHighSemitone = 12f;

        protected override void OnEnable()
        {
            base.OnEnable();
        }

        protected override void DrawTree()
        {
            // Odin 기본 드로잉 (TitleGroup, Button, ProgressBar 등)
            base.DrawTree();

            EditorGUILayout.Space(8);

            // SoundContainer Quick Setup
            DrawSoundContainerSetup();
        }

        private void DrawSoundContainerSetup()
        {
            _showSetup = EditorGUILayout.Foldout(_showSetup, "SoundContainer Quick Setup", true, EditorStyles.foldoutHeader);
            if (!_showSetup) return;

            var loopSoundProp = serializedObject.FindProperty("loopSound");
            var soundEvent = loopSoundProp?.objectReferenceValue as SoundEventBase;
            if (soundEvent == null)
            {
                EditorGUILayout.HelpBox("SoundEvent를 할당하세요.", MessageType.Info);
                return;
            }

            var containers = soundEvent.internals.soundContainers;
            if (containers == null || containers.Length == 0)
            {
                EditorGUILayout.HelpBox("SoundEvent에 SoundContainer가 없습니다.", MessageType.Warning);
                return;
            }

            EditorGUI.indentLevel++;

            // 현재 상태 표시
            for (var i = 0; i < containers.Length; i++)
            {
                var sc = containers[i];
                if (sc == null) continue;

                var data = sc.internals.data;
                EditorGUILayout.LabelField($"SC [{i}]: {sc.name}", EditorStyles.boldLabel);

                using (new EditorGUI.DisabledScope(true))
                {
                    EditorGUILayout.Toggle("Loop", data.loopEnabled);
                    EditorGUILayout.Toggle("Follow Position", data.followPosition);
                    EditorGUILayout.Toggle("Pitch Intensity", data.pitchIntensityEnable);

                    if (data.pitchIntensityEnable)
                    {
                        EditorGUI.indentLevel++;
                        EditorGUILayout.FloatField("Low (semitone)", data.pitchIntensityLowSemitone);
                        EditorGUILayout.FloatField("High (semitone)", data.pitchIntensityHighSemitone);
                        EditorGUI.indentLevel--;
                    }

                    EditorGUILayout.Toggle("Volume Intensity", data.volumeIntensityEnable);
                }

                EditorGUILayout.Space(4);
            }

            EditorGUI.indentLevel--;

            EditorGUILayout.Space(4);
            EditorGUILayout.LabelField("Apply Settings", EditorStyles.boldLabel);

            _pitchLowSemitone = EditorGUILayout.FloatField("Pitch Low (semitone)", _pitchLowSemitone);
            _pitchHighSemitone = EditorGUILayout.FloatField("Pitch High (semitone)", _pitchHighSemitone);

            EditorGUILayout.Space(4);

            // 루프 + Pitch Intensity 적용 버튼
            if (GUILayout.Button("Apply Loop + Pitch Intensity", GUILayout.Height(28)))
                ApplyLoopAndPitchIntensity(containers);

            // 루프만 적용 버튼
            if (GUILayout.Button("Apply Loop Only"))
                ApplyLoopOnly(containers);
        }

        private void ApplyLoopAndPitchIntensity(SoundContainerBase[] containers)
        {
            foreach (var sc in containers)
            {
                if (sc == null) continue;

                Undo.RecordObject(sc, "LoopSoundPlayer: Apply Loop + Pitch Intensity");

                var data = sc.internals.data;

                // 루프 기본 설정
                data.loopEnabled = true;
                data.followPosition = true;
                data.stopIfTransformIsNull = true;
                data.randomStartPosition = true;

                // Pitch Intensity
                data.pitchIntensityEnable = true;
                data.pitchIntensityRolloff = 0f;
                data.pitchIntensityCurve = AnimationCurve.Linear(0, 0, 1, 1);

                // Semitone 범위 설정
                var low = Mathf.Min(_pitchLowSemitone, _pitchHighSemitone);
                var high = Mathf.Max(_pitchLowSemitone, _pitchHighSemitone);
                data.pitchIntensityLowSemitone = low;
                data.pitchIntensityHighSemitone = high;
                data.pitchIntensityBaseSemitone = low;
                data.pitchIntensityBaseRatio = PitchScale.SemitonesToRatio(low);
                data.pitchIntensityRangeSemitone = high - low;
                data.pitchIntensityRangeRatio = PitchScale.SemitonesToRatio(high - low);
                data.pitchIntensityLowRatio = PitchScale.SemitonesToRatio(low);
                data.pitchIntensityHighRatio = PitchScale.SemitonesToRatio(high);

                EditorUtility.SetDirty(sc);
            }

            AssetDatabase.SaveAssets();
            Debug.Log($"[LoopSoundPlayer] Loop + Pitch Intensity 적용 완료 (Low: {_pitchLowSemitone}st, High: {_pitchHighSemitone}st)");
        }

        private void ApplyLoopOnly(SoundContainerBase[] containers)
        {
            foreach (var sc in containers)
            {
                if (sc == null) continue;

                Undo.RecordObject(sc, "LoopSoundPlayer: Apply Loop Only");

                var data = sc.internals.data;
                data.loopEnabled = true;
                data.followPosition = true;
                data.stopIfTransformIsNull = true;
                data.randomStartPosition = true;

                EditorUtility.SetDirty(sc);
            }

            AssetDatabase.SaveAssets();
            Debug.Log("[LoopSoundPlayer] Loop 설정 적용 완료");
        }
    }
}
