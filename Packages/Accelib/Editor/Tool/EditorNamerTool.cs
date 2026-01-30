using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using System;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

public class EditorNamerTool : EditorWindow
{
    [MenuItem("Tools/Hierarchy Namer Tool")]
    public static void OpenNamerTool()
    {
        var window = GetWindow<EditorNamerTool>();
        window.Show();
    }


    private Box leftboxGroup;
    private VisualElement topGroup;
    private ScrollView leftSelectionNodeGroup;
    private ScrollView renameTypeScroll;
    private VisualElement renameOptionWindow;
    private Box renameOptionGroup;

    private List<NamerNodeBundle> nodes;
    private Dictionary<RenameType, EnumButton<RenameType>> buttons;
    private RenameType currentType = RenameType.Prefix;

    private TextField oldTextField;
    private TextField newTextField;
    private TextField formatTextField;
    private Button applyBtn;
    private Button cancleBtn;

    private void CreateGUI()
    {
        leftboxGroup ??= new Box();
        leftSelectionNodeGroup ??= new ScrollView(ScrollViewMode.Vertical);
        renameTypeScroll ??= new ScrollView(ScrollViewMode.Vertical);
        renameOptionGroup ??= new Box();

        leftboxGroup.style.width = new StyleLength(Length.Percent(80f));
        renameTypeScroll.style.flexGrow = 1;

        topGroup = new VisualElement();
        topGroup.style.flexDirection = FlexDirection.Row;

        leftboxGroup.Add(topGroup);
        leftboxGroup.Add(leftSelectionNodeGroup);

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
        InitSelectionNodeGroup();
        InitRenameTypeGroup();
        InitRenameOptionGroup();

        rootVisualElement.Add(leftboxGroup);
        rootVisualElement.Add(renameTypeScroll);
        rootVisualElement.Add(renameOptionWindow);
        rootVisualElement.style.flexDirection = FlexDirection.Row;
    }

    public void OnEnable()
    {
        Selection.selectionChanged += CallbackSelection;
    }

    private void InitTopGroup()
    {
        var borderColor = new Color(0.65f, 0.65f, 0.65f, 1f);

        var oldLabel = new Label("Old Name");
        oldLabel.style.flexGrow = 1;
        oldLabel.style.backgroundColor = Color.gray;

        oldLabel.style.borderBottomColor = borderColor;
        oldLabel.style.borderTopColor = borderColor;
        oldLabel.style.borderRightColor = borderColor;
        oldLabel.style.borderLeftColor = borderColor;

        oldLabel.style.borderBottomWidth = 1;
        oldLabel.style.borderTopWidth = 1;
        oldLabel.style.borderRightWidth = 1;
        oldLabel.style.borderLeftWidth = 1;

        var spliteLine = new Image();

        oldLabel.style.width = new StyleLength(Length.Percent(50f));

        var newLabel = new Label("New Name");
        newLabel.style.flexGrow = 1;
        newLabel.style.backgroundColor = Color.gray;

        newLabel.style.borderBottomColor = borderColor;
        newLabel.style.borderTopColor = borderColor;
        newLabel.style.borderRightColor = borderColor;
        newLabel.style.borderLeftColor = borderColor;

        newLabel.style.borderBottomWidth = 1;
        newLabel.style.borderTopWidth = 1;
        newLabel.style.borderRightWidth = 1;
        newLabel.style.borderLeftWidth = 1;

        newLabel.style.width = new StyleLength(Length.Percent(50f));

        topGroup.Add(oldLabel);
        topGroup.Add(newLabel);
    }

    private void InitSelectionNodeGroup()
    {
        leftSelectionNodeGroup.Clear();

        nodes ??= new List<NamerNodeBundle>();
        nodes.Clear();

        var selecter = Selection.gameObjects;
        var max = selecter.Length;
        for (int idx = 0; idx < max; idx++)
        {
            var targetObject = selecter[idx];

            var labelGroup = new VisualElement();
            labelGroup.style.flexDirection = FlexDirection.Row;
            labelGroup.style.backgroundColor = idx % 2 == 0 ? Color.clear : Color.grey;


            var oldLabel = new Label(targetObject.name);
            oldLabel.style.flexGrow = 1;
            oldLabel.style.width = new StyleLength(Length.Percent(50f));

            var newLabel = new Label(targetObject.name);
            newLabel.style.flexGrow = 1;
            newLabel.style.width = new StyleLength(Length.Percent(50f));

            labelGroup.Add(oldLabel);
            labelGroup.Add(newLabel);

            leftSelectionNodeGroup.Add(labelGroup);

            nodes.Add(new NamerNodeBundle(targetObject, newLabel, oldLabel));
        }
    }

    private void InitRenameTypeGroup()
    {
        renameTypeScroll.Clear();
       
        var names = Enum.GetNames(typeof(RenameType));
        var max = names.Length;
        buttons ??= new Dictionary<RenameType, EnumButton<RenameType>>(max);

        for (int idx = 0;idx < max; idx++ )
        {
            var curType = (RenameType)idx;
            var newEnumBtn = new EnumButton<RenameType>();
            newEnumBtn.EnumValue = curType;
            newEnumBtn.text = names[idx];
            newEnumBtn.clickAction = RenameTypeClickAction;
            newEnumBtn.style.backgroundColor = Color.gray;

            buttons.TryAdd(curType, newEnumBtn);
            renameTypeScroll.Add(newEnumBtn);
        }

    }

    private void InitRenameOptionGroup()
    {
        renameOptionGroup.Clear();

        oldTextField ??= new TextField("Old Value");
        newTextField ??= new TextField("New Value");
        formatTextField ??= new TextField("Format");
        applyBtn ??= new Button(NodeUpdate);
        applyBtn.text = "Apply";

        cancleBtn ??= new Button(OffRenameOptionWindow);
        cancleBtn.text = "Cancel";

        renameOptionGroup.Add(oldTextField);
        renameOptionGroup.Add(newTextField);
        renameOptionGroup.Add(formatTextField);
        renameOptionGroup.Add(applyBtn);
        renameOptionGroup.Add(cancleBtn);
    }

    private void RenameTypeClickAction(RenameType rename)
    {
        currentType = rename;
        OnRenameOptionWindow();
    }



    private void NodeUpdate()
    {
        var max = nodes.Count;
        switch (currentType)
        {
            case RenameType.Prefix:
                for (int idx = 0; idx < max; idx++)
                    nodes[idx].newLabel.text = $"{newTextField.value}{nodes[idx].newLabel.text}";
                break;
            case RenameType.Suffix:
                for (int idx = 0; idx < max; idx++)
                    nodes[idx].newLabel.text = $"{nodes[idx].newLabel.text}{newTextField.value}";
                break;
            case RenameType.Replace:
                for (int idx = 0; idx < max; idx++)
                    nodes[idx].newLabel.text = nodes[idx].newLabel.text.Replace(oldTextField.value, newTextField.value);
                break;
            case RenameType.ClearName:
                for (int idx = 0; idx < max; idx++)
                    nodes[idx].newLabel.text = string.Empty;
                break;
            case RenameType.SuffixAddNumber:
                for (int idx = 0; idx < max; idx++)
                    nodes[idx].newLabel.text = $"{nodes[idx].newLabel.text}{string.Format(formatTextField.value, idx)}";
                break;
            case RenameType.Upper:
                for (int idx = 0; idx < max; idx++)
                    nodes[idx].newLabel.text = nodes[idx].newLabel.text.ToUpper();
                break;
            case RenameType.Lower:
                for (int idx = 0; idx < max; idx++)
                    nodes[idx].newLabel.text = nodes[idx].newLabel.text.ToLower();
                break;
            case RenameType.Trim:
                for (int idx = 0; idx < max; idx++)
                    nodes[idx].newLabel.text = nodes[idx].newLabel.text.Trim();
                break;
            case RenameType.Reset:
                for (int idx = 0; idx < max; idx++)
                    nodes[idx].newLabel.text = nodes[idx].oldLabel.text;

                break;
            case RenameType.Apply:
                for (int idx = 0; idx < max; idx++)                
                    nodes[idx].targetObject.name = nodes[idx].newLabel.text;


                UnityEngine.SceneManagement.Scene currentScene = SceneManager.GetActiveScene();
                EditorSceneManager.MarkSceneDirty(currentScene);
                break;
        }

        OffRenameOptionWindow();
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
                SetTextField(oldText: "Replace old string ", newText: "Replace new string");
                break;
            case RenameType.SuffixAddNumber:
                SetTextField(formatText: "Integer format");
                break;
            case RenameType.ClearName:
                SetDisplayDialog("Clear", "Sure Clear all name?");
                break;
            case RenameType.Upper:
                SetDisplayDialog("Upper", "Sure Upoer all name?");
                break;
            case RenameType.Lower:
                SetDisplayDialog("Lower", "Sure Lower all name?");
                break;
            case RenameType.Trim:
                SetDisplayDialog("Trim", "Sure Trim all name?");
                break;
            case RenameType.Apply:
                SetDisplayDialog("Apply", "Sure apply change name?");
                break;
            case RenameType.Reset:
                SetDisplayDialog("Reset", "Sure reset all name?");
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

    private void CallbackSelection()
    {
        InitSelectionNodeGroup();
    }

    public void OnDisable()
    {
        Selection.selectionChanged -= CallbackSelection;
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
        Apply
    }

    public class NamerNodeBundle
    {
        public GameObject targetObject;
        public Label oldLabel;
        public Label newLabel;

        public NamerNodeBundle(GameObject objects, Label newLabel, Label oldLabel)
        {
            targetObject = objects;
            this.newLabel = newLabel;
            this.oldLabel = oldLabel;
        }
    }

    public class EnumButton<T> : Button where T : struct
    {
        public T EnumValue { get; set; }

        public Action<T> clickAction;

        public EnumButton()
        {
            clicked += () =>           
                clickAction?.Invoke(EnumValue);            
        }
    }

}
