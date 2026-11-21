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
            public string Parameter;
            public float Value;
        }

        private class ButtonItem : MenuItem
        {
            public override ControlType Type => ControlType.Button;
        }

        private class ToggleItem : MenuItem
        {
            public override ControlType Type => ControlType.Toggle;
        }

        private class SubMenuItem : MenuItem
        {
            public override ControlType Type => ControlType.SubMenu;
            public VRCExpressionsMenu Asset;
            public string FilePath;
            public List<MenuItem> Items = new();

            public SubMenuItem GetSubMenu(string name) {
                int idx = Items.FindIndex(x =>
                    ControlType.SubMenu == x.Type &&
                    name.Equals(x.Name, StringComparison.Ordinal)
                );

                if (0 <= idx)
                    return (SubMenuItem)Items[idx];

                return null;
            }

            public MenuItem GetItem(string name) {
                int idx = Items.FindIndex(x =>
                    ControlType.SubMenu != x.Type &&
                    name.Equals(x.Name, StringComparison.Ordinal)
                );

                if (0 <= idx)
                    return Items[idx];

                return null;
            }
        }

        private class TwoAxisItem : MenuItem
        {
            public override ControlType Type => ControlType.Toggle;
            public string HorizontalParam;
            public string VerticalParam;
            public string UpLabel;
            public Texture2D UpIcon;
            public string RightLabel;
            public Texture2D RightIcon;
            public string DownLabel;
            public Texture2D DownIcon;
            public string LeftLabel;
            public Texture2D LeftIcon;
        }

        private class FourAxisItem : MenuItem
        {
            public override ControlType Type => ControlType.Toggle;
            public string UpParam;
            public string UpLabel;
            public Texture2D UpIcon;
            public string RightParam;
            public string RightLabel;
            public Texture2D RightIcon;
            public string DownParam;
            public string DownLabel;
            public Texture2D DownIcon;
            public string LeftParam;
            public string LeftLabel;
            public Texture2D LeftIcon;
        }

        private class RadialItem : MenuItem
        {
            public override ControlType Type => ControlType.Toggle;
            public string RadialParam;
        }

        private readonly SubMenuItem _mainMenu;

        public MenuBuilder() {
            VRCExpressionsMenu menu = ScriptableObject.CreateInstance<VRCExpressionsMenu>();
            menu.name = "Tori Menu";
            _mainMenu = new() {
                Name = menu.name,
                Asset = ScriptableObject.CreateInstance<VRCExpressionsMenu>(),
                FilePath = Path.Combine(ToriUtils.GeneratedAssetFolder, $"{menu.name}.asset")
            };
        }

        public MenuBuilder(string filePath) {
            VRCExpressionsMenu menu = ScriptableObject.CreateInstance<VRCExpressionsMenu>();
            menu.name = Path.GetFileNameWithoutExtension(filePath);
            _mainMenu = new() {
                Name = menu.name,
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

            _mainMenu = LoadMenu(menu, assetPath);
        }

        private SubMenuItem LoadMenu(VRCExpressionsMenu menu, string filePath = null, Texture2D icon = null,
            string parameter = "", float value = 1f
        ) {
            SubMenuItem menuItem = new() {
                Name = menu.name,
                Icon = icon,
                Parameter = parameter,
                Value = value,
                Asset = menu,
                FilePath = string.IsNullOrWhiteSpace(filePath)
                    ? AssetDatabase.GetAssetPath(menu)
                    : filePath
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
                    case ControlType.SubMenu: {
                        _mainMenu.Items.Add(LoadMenu(
                            item.subMenu, null, item.icon, item.parameter.name, item.value
                        ));
                        break;
                    }
                    case ControlType.TwoAxisPuppet: {
                        Parameter paramHorizontal = item.GetSubParameter(0);
                        Parameter paramVertical = item.GetSubParameter(1);

                        Label labelUp = item.GetLabel(0);
                        Label labelRight = item.GetLabel(1);
                        Label labelDown = item.GetLabel(2);
                        Label labelLeft = item.GetLabel(3);

                        _mainMenu.Items.Add(new TwoAxisItem() {
                            Name = item.name,
                            Icon = item.icon,
                            Parameter = item.parameter.name,
                            Value = item.value,
                            HorizontalParam = paramHorizontal?.name ?? string.Empty,
                            VerticalParam = paramVertical?.name ?? string.Empty,
                            UpLabel = labelUp.name ?? string.Empty,
                            UpIcon = labelUp.icon,
                            RightLabel = labelRight.name ?? string.Empty,
                            RightIcon = labelRight.icon,
                            DownLabel = labelDown.name ?? string.Empty,
                            DownIcon = labelDown.icon,
                            LeftLabel = labelLeft.name ?? string.Empty,
                            LeftIcon = labelLeft.icon
                        });
                        break;
                    }
                    case ControlType.FourAxisPuppet: {
                        Parameter paramUp = item.GetSubParameter(0);
                        Label labelUp = item.GetLabel(0);

                        Parameter paramRight = item.GetSubParameter(1);
                        Label labelRight = item.GetLabel(1);

                        Parameter paramDown = item.GetSubParameter(2);
                        Label labelDown = item.GetLabel(2);

                        Parameter paramLeft = item.GetSubParameter(3);
                        Label labelLeft = item.GetLabel(3);

                        _mainMenu.Items.Add(new FourAxisItem() {
                            Name = item.name,
                            Icon = item.icon,
                            Parameter = item.parameter.name,
                            Value = item.value,
                            UpParam = paramUp?.name ?? string.Empty,
                            UpLabel = labelUp.name ?? string.Empty,
                            UpIcon = labelUp.icon,
                            RightParam = paramRight?.name ?? string.Empty,
                            RightLabel = labelRight.name ?? string.Empty,
                            RightIcon = labelRight.icon,
                            DownParam = paramDown?.name ?? string.Empty,
                            DownLabel = labelDown.name ?? string.Empty,
                            DownIcon = labelDown.icon,
                            LeftParam = paramLeft?.name ?? string.Empty,
                            LeftLabel = labelLeft.name ?? string.Empty,
                            LeftIcon = labelLeft.icon
                        });
                        break;
                    }
                    case ControlType.RadialPuppet: {
                        Parameter param = item.GetSubParameter(0);
                        _mainMenu.Items.Add(new RadialItem() {
                            Name = item.name,
                            Icon = item.icon,
                            Parameter = item.parameter.name,
                            Value = item.value,
                            RadialParam = param?.name ?? string.Empty
                        });
                        break;
                    }
                    default: {
                        Debug.LogWarning($"[{ToriUtils.LogPrefix}] Unsupported menu item type", menu);
                        break;
                    }
                }
            }

            return menuItem;
        }

        private SubMenuItem GetParentMenu(string menuPrefix) {
            if (string.IsNullOrWhiteSpace(menuPrefix))
                return _mainMenu;

            var split = menuPrefix.Split('/');
            SubMenuItem menu = _mainMenu;
            SubMenuItem child;

            foreach (var item in split) {
                child = menu.GetSubMenu(item);

                if (null != child) {
                    menu = child;
                    continue;
                }

                string fileName = ToriUtils.MakeUniqueName(
                    $"{item}{ToriUtils.GeneratedAssetSuffix}",
                    (candidate) => {
                        var path = Path.Combine(ToriUtils.GeneratedAssetFolder, $"{candidate}.asset");
                        return null != AssetDatabase.LoadMainAssetAtPath(path);
                    }
                );
                VRCExpressionsMenu menuFile = ScriptableObject.CreateInstance<VRCExpressionsMenu>();
                menuFile.name = fileName;
                child = new() {
                    Name = item,
                    Asset = menuFile,
                    FilePath = Path.Combine(ToriUtils.GeneratedAssetFolder, $"{menuFile.name}.asset")
                };
                menu.Items.Add(child);
                menu = child;
            }

            return menu;
        }

        public void AddButton(string menuPrefix, string name, Texture2D icon = null, string parameter = "", float value = 1f) {
            string uniqueName = name;
            SubMenuItem parent = GetParentMenu(menuPrefix);
            MenuItem item = parent.GetItem(uniqueName);

            if (null != item) {
                if (ControlType.Button == item.Type && parameter.Equals(item.Parameter, StringComparison.Ordinal))
                    return;

                uniqueName = ToriUtils.MakeUniqueName(name, (candidate) => null != parent.GetItem(candidate));
            }

            parent.Items.Add(new ButtonItem() {
                Name = uniqueName,
                Icon = icon,
                Parameter = parameter,
                Value = value
            });
        }

        public void AddToggle(string menuPrefix, string name, Texture2D icon = null, string parameter = "", float value = 1f) {
            string uniqueName = name;
            SubMenuItem parent = GetParentMenu(menuPrefix);
            MenuItem item = parent.GetItem(uniqueName);

            if (null != item) {
                if (ControlType.Toggle == item.Type && parameter.Equals(item.Parameter, StringComparison.Ordinal))
                    return;

                uniqueName = ToriUtils.MakeUniqueName(name, (candidate) => null != parent.GetItem(candidate));
            }

            parent.Items.Add(new ToggleItem() {
                Name = uniqueName,
                Icon = icon,
                Parameter = parameter,
                Value = value
            });
        }

        public void AddSubmenu(string menuPrefix, string name, Texture2D icon = null, string parameter = "", float value = 1f) {
            SubMenuItem parent = GetParentMenu(menuPrefix);
            SubMenuItem menu = parent.GetSubMenu(name);

            if (null != menu)
                return;

            string fileName = ToriUtils.MakeUniqueName(
                $"{name}{ToriUtils.GeneratedAssetSuffix}",
                (candidate) => {
                    var path = Path.Combine(ToriUtils.GeneratedAssetFolder, $"{candidate}.asset");
                    return null != AssetDatabase.LoadMainAssetAtPath(path);
                }
            );
            VRCExpressionsMenu menuFile = ScriptableObject.CreateInstance<VRCExpressionsMenu>();
            menuFile.name = fileName;
            menu = new() {
                Name = name,
                Asset = menuFile,
                FilePath = Path.Combine(ToriUtils.GeneratedAssetFolder, $"{menuFile.name}.asset"),
                Icon = icon,
                Parameter = parameter,
                Value = value
            };
            parent.Items.Add(menu);
        }

        public void AddTwoAxis(string menuPrefix, string name, Texture2D icon = null, string parameter = "", float value = 1f,
            string horizontalParam = "", string verticalParam = "", string upLabel = "", Texture2D upIcon = null,
            string rightLabel = "", Texture2D rightIcon = null, string downLabel = "", Texture2D downIcon = null,
            string leftLabel = "", Texture2D leftIcon = null
        ) {
            string uniqueName = name;
            SubMenuItem parent = GetParentMenu(menuPrefix);
            MenuItem item = parent.GetItem(uniqueName);

            if (null != item) {
                if (ControlType.TwoAxisPuppet == item.Type &&
                    parameter.Equals(item.Parameter, StringComparison.Ordinal) &&
                    horizontalParam.Equals(((TwoAxisItem)item).HorizontalParam, StringComparison.Ordinal) &&
                    verticalParam.Equals(((TwoAxisItem)item).VerticalParam, StringComparison.Ordinal)
                )
                    return;

                uniqueName = ToriUtils.MakeUniqueName(name, (candidate) => null != parent.GetItem(candidate));
            }

            parent.Items.Add(new TwoAxisItem() {
                Name = uniqueName,
                Icon = icon,
                Parameter = parameter,
                Value = value,
                HorizontalParam = horizontalParam,
                VerticalParam = verticalParam,
                UpLabel = upLabel,
                UpIcon = upIcon,
                RightLabel = rightLabel,
                RightIcon = rightIcon,
                DownLabel = downLabel,
                DownIcon = downIcon,
                LeftLabel = leftLabel,
                LeftIcon = leftIcon
            });
        }

        public void AddFourAxis(string menuPrefix, string name, Texture2D icon = null, string parameter = "", float value = 1f,
            string upParam = "", string rightParam = "", string downParam = "", string leftParam = "", string upLabel = "",
            Texture2D upIcon = null, string rightLabel = "", Texture2D rightIcon = null, string downLabel = "",
            Texture2D downIcon = null, string leftLabel = "", Texture2D leftIcon = null
        ) {
            string uniqueName = name;
            SubMenuItem parent = GetParentMenu(menuPrefix);
            MenuItem item = parent.GetItem(uniqueName);

            if (null != item) {
                if (ControlType.FourAxisPuppet == item.Type &&
                    parameter.Equals(item.Parameter, StringComparison.Ordinal) &&
                    upParam.Equals(((FourAxisItem)item).UpParam, StringComparison.Ordinal) &&
                    rightParam.Equals(((FourAxisItem)item).RightParam, StringComparison.Ordinal) &&
                    downParam.Equals(((FourAxisItem)item).DownParam, StringComparison.Ordinal) &&
                    leftParam.Equals(((FourAxisItem)item).LeftParam, StringComparison.Ordinal)

                )
                    return;

                uniqueName = ToriUtils.MakeUniqueName(name, (candidate) => null != parent.GetItem(candidate));
            }

            parent.Items.Add(new FourAxisItem() {
                Name = uniqueName,
                Icon = icon,
                Parameter = parameter,
                Value = value,
                UpParam = upParam,
                UpLabel = upLabel,
                UpIcon = upIcon,
                RightParam = rightParam,
                RightLabel = rightLabel,
                RightIcon = rightIcon,
                DownParam = downParam,
                DownLabel = downLabel,
                DownIcon = downIcon,
                LeftParam = leftParam,
                LeftLabel = leftLabel,
                LeftIcon = leftIcon
            });
        }

        public void AddRadial(string menuPrefix, string name, Texture2D icon = null, string parameter = "", float value = 1f,
            string radialParam = ""
        ) {
            string uniqueName = name;
            SubMenuItem parent = GetParentMenu(menuPrefix);
            MenuItem item = parent.GetItem(uniqueName);

            if (null != item) {
                if (ControlType.RadialPuppet == item.Type &&
                    parameter.Equals(item.Parameter, StringComparison.Ordinal) &&
                    radialParam.Equals(((RadialItem)item).RadialParam, StringComparison.Ordinal)
                )
                    return;

                uniqueName = ToriUtils.MakeUniqueName(name, (candidate) => null != parent.GetItem(candidate));
            }

            parent.Items.Add(new RadialItem() {
                Name = uniqueName,
                Icon = icon,
                Parameter = parameter,
                Value = value,
                RadialParam = radialParam
            });
        }

        public void Build() {
            //
        }
    }
}
#endif
