#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using VRC.SDK3.Avatars.ScriptableObjects;
using static VRC.SDK3.Avatars.ScriptableObjects.VRCExpressionsMenu.Control;

namespace com.torinyan.PubTools.Editor.Builders
{
    internal class MenuBuilder
    {
        private abstract class MenuItem
        {
            public string Name;
            public Texture2D Icon;
            public abstract ControlType Type { get; }
        }

        private class ButtonItem : MenuItem
        {
            public override ControlType Type => ControlType.Button;
            public string Parameter;
            public float Value;
        }

        private class ToggleItem : MenuItem
        {
            public override ControlType Type => ControlType.Toggle;
            public string Parameter;
            public float Value;
        }

        private class MenuFile
        {
            public VRCExpressionsMenu Asset;
            public string FilePath;
            public List<MenuItem> Items = new();
        }

        private MenuFile _mainMenu;

        public MenuBuilder() {
            _mainMenu = new() {
                Asset = ScriptableObject.CreateInstance<VRCExpressionsMenu>(),
                FilePath = Path.Combine(ToriUtils.GeneratedAssetFolder, "Tori Menu.asset")
            };
        }

        public MenuBuilder(string filePath) {
            VRCExpressionsMenu menu = ScriptableObject.CreateInstance<VRCExpressionsMenu>();
            menu.name = "Tori Menu";
            _mainMenu = new() {
                Asset = menu,
                FilePath = filePath
            };
        }

        public MenuBuilder(VRCExpressionsMenu mainMenu) {
            VRCExpressionsMenu menu = mainMenu;
            string assetPath = AssetDatabase.GetAssetPath(menu);

            if (string.IsNullOrWhiteSpace(assetPath)) {
                menu = ScriptableObject.CreateInstance<VRCExpressionsMenu>();
                menu.name = "Tori Menu";
                assetPath = Path.Combine(ToriUtils.GeneratedAssetFolder, "Tori Menu.asset");
            } else if (!(
                assetPath.Contains(ToriUtils.CToriGeneratedFolder, StringComparison.OrdinalIgnoreCase) ||
                assetPath.Contains(ToriUtils.CVRCFuryTempFolder, StringComparison.OrdinalIgnoreCase) ||
                assetPath.Contains(ToriUtils.CNDMFGeneratedFolder, StringComparison.OrdinalIgnoreCase)
            )) {
                assetPath = Path.Combine(ToriUtils.GeneratedAssetFolder, $"{menu.name}{ToriUtils.GeneratedAssetSuffix}.asset");
            }

            LoadMenu(menu, assetPath);
        }

        private void LoadMenu(VRCExpressionsMenu menu, string filePath) {
            _mainMenu = new() {
                Asset = menu,
                FilePath = filePath
            };

            foreach (var item in menu.controls) {
                switch (item.type) {
                    case ControlType.Button: {
                        _mainMenu.Items.Add(new ButtonItem() {
                            Name = item.name,
                            Icon = item.icon,
                            Parameter = item.parameter.name,
                            Value = item.value
                        });
                        break;
                    }
                    case ControlType.Toggle: {
                        _mainMenu.Items.Add(new ToggleItem() {
                            Name = item.name,
                            Icon = item.icon,
                            Parameter = item.parameter.name,
                            Value = item.value
                        });
                        break;
                    }
                    default: {
                        Debug.LogWarning($"[{ToriUtils.LogPrefix}] Unsupported menu item type", menu);
                        break;
                    }
                }
            }
        }

        public void Build() {
            //
        }
    }
}
#endif
