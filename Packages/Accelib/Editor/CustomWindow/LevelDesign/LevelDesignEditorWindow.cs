using System.Linq;
using Accelib.Editor.CustomWindow.Core;
using Accelib.Editor.CustomWindow.LevelDesign.Model;
using Accelib.Preview;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace Accelib.Editor.CustomWindow.LevelDesign
{
    public class LevelDesignEditorWindow : OdinMenuEditorWindow
    {
        [MenuItem("Accelib/레벨디자인 매니저")]
        private static void OpenWindow() => GetWindow<LevelDesignEditorWindow>("[Accelib] 레벨디자인 매니저").Show();
        
        protected override OdinMenuTree BuildMenuTree()
        {
            var tree = new OdinMenuTree
            {
                DefaultMenuStyle = OdinMenuStyle.TreeViewStyle,
                Selection = { SupportsMultiSelect = false }
            };

            // Config 그리기
            var config = LevelDesignConfig.EditorInstance;
            if(config)
                tree.Add("옵션", config, SdfIconType.GearFill);
            else
                tree.Add("옵션", new EmptySOWindow<LevelDesignConfig>(), SdfIconType.GearFill);

            // 메뉴 그리기
            if (config)
            {
                foreach (var layout in config.MenuLayouts)
                foreach (var o in layout.dataList.Where(o => o))
                {
                    var assetName = o.name;
                    if (o is IPreviewNameProvider provider)
                        assetName = provider.EditorPreviewName;
                    if( o is IPreviewIconProvider iconProvider)
                        tree.Add($"{layout.label}/{assetName}", o, iconProvider.EditorPreviewIcon);
                    else
                        tree.Add($"{layout.label}/{assetName}", o);
                }
            }
            
            // 첫 오브젝트 선택
            var lastSelectedPath = EditorPrefs.GetString(LastSelectedPathKey, "");
            var item = tree.GetMenuItem(lastSelectedPath);
            
            // 선택지
            tree.Selection.Add(item);
            tree.Selection.SelectionChanged += OnSelectionChanged;
            
            return tree;
        }
        
        private void OnSelectionChanged(SelectionChangedType type)
        {
            if (type == SelectionChangedType.ItemAdded)
            {
                // 캐싱
                var selection = MenuTree.Selection;
                var menuItem = selection.First();
                var path = menuItem.GetFullPath();
                
                // 선택 경로 기억하기
                EditorPrefs.SetString(LastSelectedPathKey, path);
                
                // 오브젝트 핑
                // if (menuItem.Value is Object asset)
                //     EditorGUIUtility.PingObject(asset);
                
                // 열고닫기
                if (menuItem.Value is not Object)
                    menuItem.Toggled = !menuItem.Toggled;
            }
        }
        
        
        protected override void DrawMenu()
        {
            base.DrawMenu();
            
            GUILayout.FlexibleSpace();
            SirenixEditorGUI.BeginHorizontalToolbar();
            {
                GUILayout.FlexibleSpace();

                if (SirenixEditorGUI.ToolbarButton("새로고침"))
                {
                    ForceMenuTreeRebuild();
                }
            }
            SirenixEditorGUI.EndHorizontalToolbar(); 
            
        }
        
        public const string LastSelectedPathKey = nameof(LevelDesignEditorWindow) + ".lastSelectedPath";
    }
}