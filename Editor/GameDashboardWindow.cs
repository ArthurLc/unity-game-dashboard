using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.UIElements;

namespace GameDashboard.Editor
{
    public class GameDashboardWindow : EditorWindow
    {
        [MenuItem("Window/Game Dashboard/Game Dashboard %g")]
        public static void ShowWindow()
        {
            var win = GetWindow<GameDashboardWindow>();
            win.titleContent = new GUIContent("Game Dashboard", EditorGUIUtility.IconContent(GameDashboard.UNITY_ICON).image);
            win.minSize = new Vector2(750, 325);
        }

        public void CreateGUI()
        {
            VisualElement root = rootVisualElement;

            TwoPaneSplitView splitView = new TwoPaneSplitView(0, 300, TwoPaneSplitViewOrientation.Horizontal);
            var leftPane = new VisualElement();
            var rightPane = new VisualElement();

            BuildHierarchyTree(leftPane, rightPane);

            splitView.Add(leftPane);
            splitView.Add(rightPane);

            root.Add(splitView);
        }

        private void BuildHierarchyTree(in VisualElement tree, VisualElement containt)
        {
            var treeView = new TreeView
            {
                style = { backgroundColor = new Color(1f, 1f, 1f, 0.03f) },
                selectionType = SelectionType.Single,
                fixedItemHeight = 28
            };

            // Define how to create a visual for each element
            treeView.makeItem = () =>
            {
                var container = new VisualElement
                {
                    style =
                    {
                        flexDirection = FlexDirection.Row,
                        alignItems = Align.Center,
                        height = Length.Percent(100)
                    }
                };

                var icon = new Image
                {
                    name = "icon",
                    style =
                    {
                        width = 16,
                        height = 16,
                        marginRight = 4
                    }
                };
                var label = new Label
                {
                    name = "label"
                };

                container.Add(icon);
                container.Add(label);

                return container;
            };

            // Define how to bind data
            treeView.bindItem = (element, index) =>
            {
                var item = treeView.GetItemDataForIndex<object>(index);

                var label = element.Q<Label>("label");
                var icon = element.Q<Image>("icon");

                if (item is ScriptableObject so)
                {
                    label.text = so.name;
                    icon.image = GameDashboardSettings.instance.GetScriptableIcon(so);
                }
                else if (item is string folderName)
                {
                    label.text = folderName;

                    int id = treeView.GetIdForIndex(index);
                    if (treeView.IsExpanded(id))
                        icon.image = EditorGUIUtility.IconContent("FolderOpened Icon").image;
                    else
                        icon.image = EditorGUIUtility.IconContent("Folder Icon").image;
                }
            };

            var rootItems = GenerateTreeFromAssets(GameDashboardSettings.instance.TreeAssetPaths);

            treeView.SetRootItems(rootItems);

            treeView.selectionChanged += (selectedIds) =>
            {
                if (selectedIds.Count() > 0)
                {
                    DrawSelection(containt, treeView.selectedItem);
                }
            };

            tree.Add(treeView);
        }

        private void DrawSelection(in VisualElement root, object selectedItem)
        {
            root.Clear();

            root.style.paddingLeft = 5;
            root.style.paddingRight = 5;
            root.style.paddingTop = 1;
            root.style.paddingBottom = 1;

            if (selectedItem is ScriptableObject so)
            {
                var editor = UnityEditor.Editor.CreateEditor(so);

                if (editor != null && HasCustomEditor(editor.GetType()))
                {
                    if (HasUIToolkitInspector(editor))
                    {
                        root.Add(editor.CreateInspectorGUI());
                    }
                    else
                    {
                        var imguiContainer = new IMGUIContainer(() =>
                        {
                            editor.OnInspectorGUI();
                        });

                        root.Add(imguiContainer);
                    }
                }
                else
                {
                    root.style.paddingLeft = 15;

                    var serializedObject = new SerializedObject(so);

                    var propertyIterator = serializedObject.GetIterator();
                    propertyIterator.Next(true);

                    while (propertyIterator.NextVisible(false))
                    {
                        var field = new PropertyField(propertyIterator.Copy());

                        if (field.bindingPath == "m_Script")
                            continue;

                        field.Bind(serializedObject);
                        root.Add(field);
                    }
                }
            }
            else
            {
                root.Add(null);
            }
        }

        #region Tree
        private List<TreeViewItemData<object>> GenerateTreeFromAssets(string[] rootPaths)
        {
            var rootChildren = new List<TreeViewItemData<object>>();
            int idCounter = 1;

            foreach (string rootPath in rootPaths)
            {
                if (!AssetDatabase.AssetPathExists(rootPath))
                    continue;

                if (AssetDatabase.IsValidFolder(rootPath))
                {
                    string rootFolderName = System.IO.Path.GetFileName(rootPath);

                    var rootFolderList = new List<TreeViewItemData<object>>();
                    var rootFolderNode = new TreeViewItemData<object>(idCounter++, rootFolderName, rootFolderList);

                    rootChildren.Add(rootFolderNode);

                    Dictionary<string, List<TreeViewItemData<object>>> pathToNode = new Dictionary<string, List<TreeViewItemData<object>>>
                    {
                        { rootPath, rootFolderList }
                    };

                    string[] guids = AssetDatabase.FindAssets("t:ScriptableObject", new[] { rootPath });
                    foreach (var guid in guids)
                    {
                        var assetPath = AssetDatabase.GUIDToAssetPath(guid);
                        var so = AssetDatabase.LoadAssetAtPath<ScriptableObject>(assetPath);
                        if (so == null)
                            continue;

                        string relativePath = assetPath.Substring(rootPath.Length).TrimStart('/');

                        var folders = relativePath.Split('/');
                        string currentPath = rootPath;
                        List<TreeViewItemData<object>> currentList = pathToNode[rootPath];

                        for (int i = 0; i < folders.Length; i++)
                        {
                            string folderOrAssetName = folders[i];
                            bool isAsset = (i == folders.Length - 1);
                            currentPath = currentPath + "/" + folderOrAssetName;

                            if (!pathToNode.ContainsKey(currentPath))
                            {
                                if (isAsset)
                                {
                                    var soNode = new TreeViewItemData<object>(idCounter++, so);
                                    currentList.Add(soNode);
                                }
                                else
                                {
                                    var newFolderList = new List<TreeViewItemData<object>>();
                                    var folderNode = new TreeViewItemData<object>(idCounter++, folderOrAssetName, newFolderList);

                                    currentList.Add(folderNode);
                                    pathToNode[currentPath] = newFolderList;
                                    currentList = newFolderList;
                                }
                            }
                            else
                            {
                                currentList = pathToNode[currentPath];
                            }
                        }
                    }
                }
                else
                {
                    var asset = AssetDatabase.LoadAssetAtPath<ScriptableObject>(rootPath);
                    if (asset != null)
                    {
                        rootChildren.Add(new TreeViewItemData<object>(asset.GetInstanceID(), AssetDatabase.LoadAssetAtPath<ScriptableObject>(rootPath)));
                    }
                }
            }

            return rootChildren;
        }
        #endregion

        #region Misc
        public static bool HasCustomEditor(Type targetType)
        {
#if EDITORATTRIBUTES_2_6_0
            return targetType != typeof(EditorAttributes.Editor.EditorExtension);
#else
            string editorTypeName = targetType.Name;
            return editorTypeName != "GenericInspector" && editorTypeName != "DefaultEditor";
#endif
        }
        public static bool HasUIToolkitInspector(UnityEditor.Editor editor)
        {
            var method = editor.GetType().GetMethod("CreateInspectorGUI",
                System.Reflection.BindingFlags.Instance |
                System.Reflection.BindingFlags.Public |
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.DeclaredOnly);

            return method != null && method.DeclaringType != typeof(UnityEditor.Editor);
        }
        #endregion
    }
}
