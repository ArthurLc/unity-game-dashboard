using UnityEngine;
using UnityEditor;
#if UNITY_6000_3_OR_NEWER
using UnityEditor.Toolbars;
#elif TOOLBAREXTENDER_1_4_2
using UnityToolbarExtender;
#endif

namespace GameDashboard.Editor
{
#if !UNITY_6000_3_OR_NEWER && TOOLBAREXTENDER_1_4_2
    [InitializeOnLoad]
#endif
    internal class GameDashboardShortcutButton : UnityEditor.Editor
    {
#if UNITY_6000_3_OR_NEWER
        [MainToolbarElement("Game Dashboard", defaultDockPosition = MainToolbarDockPosition.Middle)]
        public static MainToolbarElement ToolbarButton()
        {
            var icon = EditorGUIUtility.IconContent(GameDashboard.UNITY_ICON).image as Texture2D;
            var content = new MainToolbarContent(icon, "Game Dashboard");
            return new MainToolbarButton(content, () => { GameDashboardWindow.ShowWindow(); });
        }
#elif TOOLBAREXTENDER_1_4_2
        private static class ToolbarStyles
        {
            public static readonly GUIStyle commandButtonStyle;

            static ToolbarStyles()
            {
                commandButtonStyle = new GUIStyle("Command")
                {
                    imagePosition = ImagePosition.ImageOnly
                };
            }
        }

        static GameDashboardShortcutButton()
        {
            ToolbarExtender.LeftToolbarGUI.Add(OnToolbarGUI);
        }

        private static void OnToolbarGUI()
        {
            GUILayout.FlexibleSpace();

            if (GUILayout.Button(new GUIContent(EditorGUIUtility.IconContent(GameDashboard.UNITY_ICON).image, "Game Dashboard"), ToolbarStyles.commandButtonStyle))
            {
                GameDashboardWindow.ShowWindow();
            }
        }
#endif
    }
}
