// EditorAssetNamerTool.cs
// 선택한 Project 에셋(파일/폴더) 이름을 일괄 변경하는 EditorWindow.
// - Project 창에서 에셋 선택 → Tools/Asset Namer Tool 열기 → 규칙 선택/입력 → Apply Rename
// - 폴더 선택 시: 하위 포함 여부 옵션 제공
// - 안전장치: "적용" 전에는 미리보기 라벨만 변경, Apply Rename 시 실제 파일명 변경(AssetDatabase.RenameAsset)
// - 주의: AssetDatabase.RenameAsset는 "에셋 이름(파일명)"만 바꿉니다(내부 오브젝트 이름과 다를 수 있음).

using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Accelib.Editor.Tool
{
    public class EditorAssetNamerTool : EditorWindow
    {
        [MenuItem("Tools/Asset Namer Tool")]
        public static void Open()
        {
            var window = GetWindow<EditorAssetNamerTool>();
            window.titleContent = new GUIContent("Asset Namer Tool");
            window.Show();
        }

        private Box leftBoxGroup;
        private VisualElement topGroup;
        private ScrollView leftSelectionNodeGroup;
        private ScrollView renameTypeScroll;
        private VisualElement renameOptionWindow;
        private Box renameOptionGroup;

        private readonly List<NodeBundle> nodes = new();
        private Dictionary<RenameType, EnumButton<RenameType>> buttons;
        private RenameType currentType = RenameType.Prefix;

        private TextField oldTextField;
        private TextField newTextField;
        private TextField formatTextField;
        private Toggle includeSubfoldersToggle;
        private Button applyBtn;
        private Button cancelBtn;

        private void CreateGUI()
        {
            leftBoxGroup ??= new Box();
            leftSelectionNodeGroup ??= new ScrollView(ScrollViewMode.Vertical);
            renameTypeScroll ??= new ScrollView(ScrollViewMode.Vertical);
            renameOptionGroup ??= new Box();

            leftBoxGroup.style.width = new StyleLength(Length.Percent(80f));
            renameTypeScroll.style.flexGrow = 1;

            topGroup = new VisualElement();
            topGroup.style.flexDirection = FlexDirection.Row;

            leftBoxGroup.Add(topGroup);
            leftBoxGroup.Add(leftSelectionNodeGroup);

            renameOptionWindow ??= new VisualElement();
            renameOptionWindow.style.position = Position.Absolute;
            renameOptionWindow.style.backgroundColor = new Color(0.5f, 0.5f, 0.5f, 0.95f);
            renameOptionWindow.style.width = new StyleLength(Length.Percent(100f));
            renameOptionWindow.style.height = new StyleLength(Length.Percent(100f));
            renameOptionWindow.style.paddingBottom = new StyleLength(Length.Percent(15f));
            renameOptionWindow.style.paddingLeft = new StyleLength(Length.Percent(15f));
            renameOptionWindow.style.paddingRight = new StyleLength(Length.Percent(15f));
            renameOptionWindow.style.paddingTop = new StyleLength(Length.Percent(15f));
            renameOptionWindow.style.display = DisplayStyle.None;

            renameOptionWindow.Add(renameOptionGroup);

            InitTopGroup();
            RefreshSelectionNodes();
            InitRenameTypeGroup();
            InitRenameOptionGroup();

            rootVisualElement.Clear();
            rootVisualElement.Add(leftBoxGroup);
            rootVisualElement.Add(renameTypeScroll);
            rootVisualElement.Add(renameOptionWindow);
            rootVisualElement.style.flexDirection = FlexDirection.Row;
        }

        private void OnEnable()
        {
            Selection.selectionChanged += OnSelectionChanged;
            EditorApplication.projectChanged += OnProjectChanged;
        }

        private void OnDisable()
        {
            Selection.selectionChanged -= OnSelectionChanged;
            EditorApplication.projectChanged -= OnProjectChanged;
        }

        private void OnSelectionChanged() => RefreshSelectionNodes();
        private void OnProjectChanged() => RefreshSelectionNodes();

        private void InitTopGroup()
        {
            topGroup.Clear();

            var borderColor = new Color(0.65f, 0.65f, 0.65f, 1f);

            var oldLabel = new Label("Old Asset Name");
            oldLabel.style.flexGrow = 1;
            oldLabel.style.backgroundColor = Color.gray;
            SetBorder(oldLabel, borderColor);
            oldLabel.style.width = new StyleLength(Length.Percent(50f));

            var newLabel = new Label("New Asset Name");
            newLabel.style.flexGrow = 1;
            newLabel.style.backgroundColor = Color.gray;
            SetBorder(newLabel, borderColor);
            newLabel.style.width = new StyleLength(Length.Percent(50f));

            topGroup.Add(oldLabel);
            topGroup.Add(newLabel);

            static void SetBorder(VisualElement ve, Color c)
            {
                ve.style.borderBottomColor = c;
                ve.style.borderTopColor = c;
                ve.style.borderRightColor = c;
                ve.style.borderLeftColor = c;

                ve.style.borderBottomWidth = 1;
                ve.style.borderTopWidth = 1;
                ve.style.borderRightWidth = 1;
                ve.style.borderLeftWidth = 1;
            }
        }

        private void RefreshSelectionNodes()
        {
            leftSelectionNodeGroup.Clear();
            nodes.Clear();

            // UI 생성 시점 전 호출 방지
            if (leftSelectionNodeGroup == null) return;

            // 폴더 포함 여부는 UI 토글 값으로 결정 (기본 false)
            bool includeSubfolders = includeSubfoldersToggle != null && includeSubfoldersToggle.value;

            var selectedPaths = GetSelectedAssetPaths(includeSubfolders);

            for (int idx = 0; idx < selectedPaths.Count; idx++)
            {
                var path = selectedPaths[idx];
                var fileName = Path.GetFileNameWithoutExtension(path);

                var row = new VisualElement();
                row.style.flexDirection = FlexDirection.Row;
                row.style.backgroundColor = idx % 2 == 0 ? Color.clear : Color.grey;

                var oldLabel = new Label(fileName);
                oldLabel.style.flexGrow = 1;
                oldLabel.style.width = new StyleLength(Length.Percent(50f));

                var newLabel = new Label(fileName);
                newLabel.style.flexGrow = 1;
                newLabel.style.width = new StyleLength(Length.Percent(50f));

                row.Add(oldLabel);
                row.Add(newLabel);

                leftSelectionNodeGroup.Add(row);

                nodes.Add(new NodeBundle(path, oldLabel, newLabel));
            }
        }

        private void InitRenameTypeGroup()
        {
            renameTypeScroll.Clear();

            var names = Enum.GetNames(typeof(RenameType));
            buttons = new Dictionary<RenameType, EnumButton<RenameType>>(names.Length);

            for (int idx = 0; idx < names.Length; idx++)
            {
                var curType = (RenameType)idx;
                var btn = new EnumButton<RenameType>
                {
                    EnumValue = curType,
                    text = names[idx],
                    clickAction = RenameTypeClickAction
                };
                btn.style.backgroundColor = Color.gray;

                buttons.TryAdd(curType, btn);
                renameTypeScroll.Add(btn);
            }
        }

        private void InitRenameOptionGroup()
        {
            renameOptionGroup.Clear();

            includeSubfoldersToggle ??= new Toggle("Include Subfolders (when folder selected)") { value = false };
            includeSubfoldersToggle.RegisterValueChangedCallback(_ => RefreshSelectionNodes());

            oldTextField ??= new TextField("Old Value");
            newTextField ??= new TextField("New Value");
            formatTextField ??= new TextField("Format (e.g. {0:000})");

            applyBtn ??= new Button(NodeUpdate) { text = "Apply" };
            cancelBtn ??= new Button(OffRenameOptionWindow) { text = "Cancel" };

            renameOptionGroup.Add(includeSubfoldersToggle);
            renameOptionGroup.Add(oldTextField);
            renameOptionGroup.Add(newTextField);
            renameOptionGroup.Add(formatTextField);
            renameOptionGroup.Add(applyBtn);
            renameOptionGroup.Add(cancelBtn);
        }

        private void RenameTypeClickAction(RenameType rename)
        {
            currentType = rename;
            OnRenameOptionWindow();
        }

        private void NodeUpdate()
        {
            int max = nodes.Count;

            // 미리보기 변경
            switch (currentType)
            {
                case RenameType.Prefix:
                    for (int i = 0; i < max; i++)
                        nodes[i].newLabel.text = $"{newTextField.value}{nodes[i].newLabel.text}";
                    break;

                case RenameType.Suffix:
                    for (int i = 0; i < max; i++)
                        nodes[i].newLabel.text = $"{nodes[i].newLabel.text}{newTextField.value}";
                    break;

                case RenameType.Replace:
                    for (int i = 0; i < max; i++)
                        nodes[i].newLabel.text = nodes[i].newLabel.text.Replace(oldTextField.value, newTextField.value);
                    break;

                case RenameType.ClearName:
                    for (int i = 0; i < max; i++)
                        nodes[i].newLabel.text = string.Empty;
                    break;

                case RenameType.SuffixAddNumber:
                    for (int i = 0; i < max; i++)
                        nodes[i].newLabel.text = $"{nodes[i].newLabel.text}{string.Format(formatTextField.value, i)}";
                    break;

                case RenameType.Upper:
                    for (int i = 0; i < max; i++)
                        nodes[i].newLabel.text = nodes[i].newLabel.text.ToUpperInvariant();
                    break;

                case RenameType.Lower:
                    for (int i = 0; i < max; i++)
                        nodes[i].newLabel.text = nodes[i].newLabel.text.ToLowerInvariant();
                    break;

                case RenameType.Trim:
                    for (int i = 0; i < max; i++)
                        nodes[i].newLabel.text = nodes[i].newLabel.text.Trim();
                    break;

                case RenameType.Reset:
                    for (int i = 0; i < max; i++)
                        nodes[i].newLabel.text = nodes[i].oldLabel.text;
                    break;

                case RenameType.ApplyRename:
                    ApplyRenameToAssets();
                    break;
            }

            OffRenameOptionWindow();
        }

        private void ApplyRenameToAssets()
        {
            if (nodes.Count == 0)
            {
                EditorUtility.DisplayDialog("Asset Namer Tool", "No assets selected.", "OK");
                return;
            }

            // 중복/빈이름 체크
            var used = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            for (int i = 0; i < nodes.Count; i++)
            {
                var bundle = nodes[i];

                var desiredName = bundle.newLabel.text ?? string.Empty;
                desiredName = desiredName.Trim();

                if (string.IsNullOrEmpty(desiredName))
                {
                    EditorUtility.DisplayDialog("Invalid Name", $"Empty name at index {i}.", "OK");
                    return;
                }

                // 파일 시스템 금지 문자 대체
                desiredName = SanitizeFileName(desiredName);

                var dir = Path.GetDirectoryName(bundle.assetPath)?.Replace('\\', '/') ?? "Assets";
                var ext = Path.GetExtension(bundle.assetPath);
                var desiredPath = $"{dir}/{desiredName}{ext}".Replace('\\', '/');

                // 같은 폴더 내 충돌 방지
                // (현재 자신과 동일 경로는 허용)
                if (!string.Equals(desiredPath, bundle.assetPath, StringComparison.OrdinalIgnoreCase))
                {
                    if (AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(desiredPath) != null)
                    {
                        EditorUtility.DisplayDialog(
                            "Name Conflict",
                            $"Target path already exists:\n{desiredPath}",
                            "OK"
                        );
                        return;
                    }
                }

                // 동일 작업 내에서도 중복 방지
                if (!used.Add(desiredPath))
                {
                    EditorUtility.DisplayDialog(
                        "Name Conflict",
                        $"Duplicate target name/path in this operation:\n{desiredPath}",
                        "OK"
                    );
                    return;
                }

                bundle.pendingSanitizedName = desiredName;
            }

            AssetDatabase.StartAssetEditing();
            try
            {
                for (int i = 0; i < nodes.Count; i++)
                {
                    var bundle = nodes[i];

                    // 폴더는 RenameAsset 가능
                    var error = AssetDatabase.RenameAsset(bundle.assetPath, bundle.pendingSanitizedName);
                    if (!string.IsNullOrEmpty(error))
                    {
                        Debug.LogError($"Rename failed: {bundle.assetPath} -> {bundle.pendingSanitizedName}\n{error}");
                    }
                }
            }
            finally
            {
                AssetDatabase.StopAssetEditing();
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }

            RefreshSelectionNodes();
        }

        private void OnRenameOptionWindow()
        {
            oldTextField.value = string.Empty;
            newTextField.value = string.Empty;
            formatTextField.value = string.Empty;

            switch (currentType)
            {
                case RenameType.Prefix:
                    SetTextField(newText: "Prefix string");
                    break;

                case RenameType.Suffix:
                    SetTextField(newText: "Suffix string");
                    break;

                case RenameType.Replace:
                    SetTextField(oldText: "Replace old string", newText: "Replace new string");
                    break;

                case RenameType.SuffixAddNumber:
                    SetTextField(formatText: "Integer format");
                    break;

                case RenameType.ClearName:
                    SetDisplayDialog("Clear", "Clear all preview names?");
                    break;

                case RenameType.Upper:
                    SetDisplayDialog("Upper", "Uppercase all preview names?");
                    break;

                case RenameType.Lower:
                    SetDisplayDialog("Lower", "Lowercase all preview names?");
                    break;

                case RenameType.Trim:
                    SetDisplayDialog("Trim", "Trim all preview names?");
                    break;

                case RenameType.ApplyRename:
                    SetDisplayDialog("Apply Rename", "Rename selected assets in Project?");
                    break;

                case RenameType.Reset:
                    SetDisplayDialog("Reset", "Reset all preview names?");
                    break;
            }

            void SetTextField(string oldText = null, string newText = null, string formatText = null)
            {
                renameOptionWindow.style.display = DisplayStyle.Flex;

                oldTextField.label = oldText;
                oldTextField.style.display = string.IsNullOrEmpty(oldText) ? DisplayStyle.None : DisplayStyle.Flex;

                newTextField.label = newText;
                newTextField.style.display = string.IsNullOrEmpty(newText) ? DisplayStyle.None : DisplayStyle.Flex;

                formatTextField.label = formatText;
                formatTextField.style.display = string.IsNullOrEmpty(formatText) ? DisplayStyle.None : DisplayStyle.Flex;
            }

            void SetDisplayDialog(string title, string content)
            {
                renameOptionWindow.style.display = DisplayStyle.None;

                oldTextField.style.display = DisplayStyle.None;
                newTextField.style.display = DisplayStyle.None;
                formatTextField.style.display = DisplayStyle.None;

                if (EditorUtility.DisplayDialog(title, content, "Apply", "Cancel"))
                    NodeUpdate();
            }
        }

        private void OffRenameOptionWindow()
        {
            renameOptionWindow.style.display = DisplayStyle.None;
        }

        private static List<string> GetSelectedAssetPaths(bool includeSubfolders)
        {
            var results = new List<string>();

            // Project 선택 대상
            var guids = Selection.assetGUIDs;
            if (guids == null || guids.Length == 0) return results;

            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                if (string.IsNullOrEmpty(path)) continue;

                if (AssetDatabase.IsValidFolder(path))
                {
                    if (!includeSubfolders)
                    {
                        // 폴더 자체만
                        results.Add(path);
                    }
                    else
                    {
                        // 폴더 하위 전부(폴더 내 에셋) + 폴더 자체는 제외(원하면 포함하도록 변경 가능)
                        var found = AssetDatabase.FindAssets("", new[] { path });
                        foreach (var fGuid in found)
                        {
                            var fPath = AssetDatabase.GUIDToAssetPath(fGuid);
                            if (string.IsNullOrEmpty(fPath)) continue;
                            if (AssetDatabase.IsValidFolder(fPath)) continue; // 기본: 하위 폴더 이름까지 바꾸지 않음
                            results.Add(fPath);
                        }
                    }
                }
                else
                {
                    results.Add(path);
                }
            }

            // 안정적으로 정렬
            results.Sort(StringComparer.OrdinalIgnoreCase);
            return results;
        }

        private static string SanitizeFileName(string name)
        {
            // Windows 기준 invalid chars 포함(유니티 프로젝트는 대부분 Windows 호환 필요)
            var invalid = Path.GetInvalidFileNameChars();
            foreach (var c in invalid)
                name = name.Replace(c, '_');

            // 끝 점/공백 등도 파일시스템 이슈가 될 수 있어 방지
            name = name.Trim().TrimEnd('.');
            return name;
        }

        public enum RenameType
        {
            Prefix,
            Suffix,
            Replace,
            ClearName,
            SuffixAddNumber,
            Upper,
            Lower,
            Trim,
            Reset,
            ApplyRename
        }

        private sealed class NodeBundle
        {
            public string assetPath;
            public Label oldLabel;
            public Label newLabel;

            // Apply 시점에 sanitize된 이름 저장
            public string pendingSanitizedName;

            public NodeBundle(string assetPath, Label oldLabel, Label newLabel)
            {
                this.assetPath = assetPath;
                this.oldLabel = oldLabel;
                this.newLabel = newLabel;
            }
        }

        private class EnumButton<T> : Button where T : struct
        {
            public T EnumValue { get; set; }
            public Action<T> clickAction;

            public EnumButton()
            {
                clicked += () => clickAction?.Invoke(EnumValue);
            }
        }
    }
}
