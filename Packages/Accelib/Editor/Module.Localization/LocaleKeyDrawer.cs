#if UNITY_EDITOR

using System.Collections.Generic;
using System.Linq;
using Accelib.Module.Localization.Model;
using UnityEditor;
using UnityEngine;

namespace Accelib.Editor.Module.Localization
{
    [CustomPropertyDrawer(typeof(LocaleKey), true)]
    public class LocaleKeyDrawer : PropertyDrawer
    {
        private const float ArrowWidth = 16f;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float line = EditorGUIUtility.singleLineHeight;
            float v = EditorGUIUtility.standardVerticalSpacing;

            if (!property.isExpanded) return line;

            var previewProp = property.FindPropertyRelative("_preview");
            int count = (previewProp != null && previewProp.isArray) ? previewProp.arraySize : 0;

            float height = line + v + line;           // key 줄 + v + "Preview" 헤더
            height += Mathf.Max(1, count) * (line + v); // 항목 또는 (empty)
            height += 2;
            return height;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            float line = EditorGUIUtility.singleLineHeight;
            float v = EditorGUIUtility.standardVerticalSpacing;

            var keyProp     = property.FindPropertyRelative("key");
            var previewProp = property.FindPropertyRelative("_preview");

            var arrowRect = new Rect(position.x, position.y, ArrowWidth, line);
            var keyRect   = new Rect(position.x + ArrowWidth, position.y, position.width - ArrowWidth, line);

            // key 필드
            EditorGUI.BeginChangeCheck();
            string newKey = EditorGUI.TextField(keyRect, keyProp.stringValue);
            bool keyChanged = EditorGUI.EndChangeCheck();

            // 폴드아웃
            property.isExpanded = EditorGUI.Foldout(arrowRect, property.isExpanded, GUIContent.none, true);

            // 🔸 key가 바뀌었으면 preview 갱신
            if (keyChanged)
            {
                // 멀티-오브젝트 편집 대응
                foreach (var target in property.serializedObject.targetObjects)
                {
                    
                }

                keyProp.stringValue = newKey;

                if (previewProp != null && previewProp.isArray)
                {
                    // 실제 프로젝트에 맞게, 여기서 로케일 테이블을 조회해 미리보기를 생성하세요.
                    // 아래는 예시: key → 다국어 샘플 문자열
                    var previews = GeneratePreviewSamples(newKey);

                    // 배열 덮어쓰기
                    SetStringArray(previewProp, previews);
                }

                // 변경 적용
                property.serializedObject.ApplyModifiedProperties();
                // 에셋/씬 더티 처리(필요 시)
                EditorUtility.SetDirty(property.serializedObject.targetObject);
            }

            // 접힘이면 종료
            if (!property.isExpanded)
            {
                EditorGUI.EndProperty();
                return;
            }

            // 펼친 상태: "Preview" 리스트 그리기
            float y = position.y + line + v;

            Rect headerRect = new Rect(position.x, y, position.width, line);
            EditorGUI.LabelField(headerRect, "Preview", EditorStyles.boldLabel);
            y += line + v;

            int count = (previewProp != null && previewProp.isArray) ? previewProp.arraySize : 0;

            if (count == 0)
            {
                using (new EditorGUI.DisabledScope(true))
                {
                    Rect itemRect = new Rect(position.x, y, position.width, line);
                    EditorGUI.TextField(itemRect, "— (empty) —");
                    y += line + v;
                }
            }
            else
            {
                using (new EditorGUI.DisabledScope(true))
                {
                    for (int i = 0; i < count; i++)
                    {
                        var elem = previewProp.GetArrayElementAtIndex(i);
                        string text = elem.propertyType == SerializedPropertyType.String
                            ? elem.stringValue
                            : elem.ToString();

                        Rect itemRect = new Rect(position.x, y, position.width, line);
                        EditorGUI.TextField(itemRect, text);
                        y += line + v;
                    }
                }
            }

            EditorGUI.EndProperty();
        }

        // ========== Helpers ==========

        // 예시 프리뷰 생성 로직: 실제로는 로케일 DB/테이블에서 가져오세요.
        private static IEnumerable<string> GeneratePreviewSamples(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                return Enumerable.Empty<string>();

            // 예: key로 각 언어 값을 조회했다고 가정
            // ex) LocaleDB.Get("ko", key) ...
            return new[]
            {
                $"ko-KR : {key} (예시 값)",
                $"en-US : {key} (sample value)",
                $"ja-JP : {key} (サンプル)"
            };
        }

        // SerializedProperty(string 배열) 덮어쓰기
        private static void SetStringArray(SerializedProperty arrayProp, IEnumerable<string> values)
        {
            // 초기화
            arrayProp.arraySize = 0;

            foreach (var s in values)
            {
                int idx = arrayProp.arraySize;
                arrayProp.InsertArrayElementAtIndex(idx);
                var elem = arrayProp.GetArrayElementAtIndex(idx);
                elem.stringValue = s ?? string.Empty;
            }

            // 값이 없으면 배열 0 유지 (OnGUI에서 empty 처리)
        }
    }
}

#endif
