using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

using Object = UnityEngine.Object;

namespace GameDashboard.Editor
{
    [FilePath("ProjectSettings/Packages/com.ArthurLc.unity-game-dashboard/GameDashboardSettings.asset", FilePathAttribute.Location.ProjectFolder)]
    internal class GameDashboardSettings : ScriptableSingleton<GameDashboardSettings>
    {
        [Serializable]
        internal struct TreeAssetEntry
        {
            public Object keyObject; // ScriptableObject or folder
            public string value; // Unity Icon
        }
        [Serializable]
        internal struct TreeAsset
        {
            public string path;
            public string icon;
        }
        internal Dictionary<string, TreeAsset> cachedTreeAssets = null;
        internal Dictionary<string, TreeAsset> TreeAssets
        {
            get
            {
                if (cachedTreeAssets == null)
                {
                    cachedTreeAssets = TreeAssetsList.ToDictionary(
                        entry => entry.keyObject != null ? entry.keyObject.name : "",
                        entry => new TreeAsset { path = entry.keyObject != null ? AssetDatabase.GetAssetPath(entry.keyObject) : "", icon = entry.value }
                    );
                }

                return cachedTreeAssets;
            }
        }
        public string[] TreeAssetPaths => TreeAssets.Select(asset => asset.Value.path).ToArray();

        public void SaveSettings() => Save(true);

        [SerializeField] private List<TreeAssetEntry> treeAssetsList;
        internal List<TreeAssetEntry> TreeAssetsList
        {
            get
            {
                if (treeAssetsList == null)
                {
                    treeAssetsList = new List<TreeAssetEntry>();
                }
                return treeAssetsList;
            }
        }

        public Texture GetScriptableIcon(ScriptableObject so)
        {
            if (TreeAssets == null || so == null)
                return null;

            if (TreeAssets.TryGetValue(so.name, out TreeAsset value) && !string.IsNullOrEmpty(value.icon))
            {
                return EditorGUIUtility.IconContent(value.icon).image;
            }
            else
            {
                return EditorGUIUtility.IconContent("ScriptableObject Icon").image;
            }
        }


        [InitializeOnLoadMethod]
        private static void InitializeOnLoad()
        {
            GetOrCreateSettings();
        }

        internal static GameDashboardSettings GetOrCreateSettings()
        {
            var settings = instance;
            if (!System.IO.File.Exists(GetFilePath()))
            {
                settings.treeAssetsList = new List<TreeAssetEntry>();
                settings.Save(true);
            }
            return settings;
        }
    }
}
