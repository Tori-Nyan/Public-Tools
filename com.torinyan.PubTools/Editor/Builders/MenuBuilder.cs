/*
* Torinyan Public Tools
* Copyright (C) 2026  Torinyan
*
* This program is free software: you can redistribute it and/or modify
* it under the terms of the GNU General Public License as published by
* the Free Software Foundation, either version 3 of the License, or
* (at your option) any later version.
*
* This program is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
* GNU General Public License for more details.
*
* You should have received a copy of the GNU General Public License
* along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/
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
            public List<MenuItem> Items = new();
            public bool IsMoreMenu = false;

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

            public void SetIcon(string name, Texture2D icon) {
                int idx = Items.FindIndex(x => name.Equals(x.Name, StringComparison.Ordinal));

                if (idx < 0)
                    return;

                var item = Items[idx];
                item.Icon = icon;
                Items[idx] = item;
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

        private SubMenuItem _mainMenu;

        public MenuBuilder() =>
            Init(null);

        public MenuBuilder(string assetPath) =>
            Init(AssetDatabase.LoadAssetAtPath<VRCExpressionsMenu>(assetPath));

        public MenuBuilder(VRCExpressionsMenu mainMenu) =>
            Init(mainMenu);

        private void Init(VRCExpressionsMenu mainMenu) {
            VRCExpressionsMenu menu = mainMenu;
            string assetPath = null == menu ? string.Empty : AssetDatabase.GetAssetPath(menu);

            if (string.IsNullOrWhiteSpace(assetPath)) {
                menu = ScriptableObject.CreateInstance<VRCExpressionsMenu>();
                menu.name = "Tori Menu";
                assetPath = Path.Combine(ToriUtils.GeneratedAssetFolder, $"{menu.name}.asset");
                AssetDatabase.CreateAsset(menu, assetPath);
            } else if (!(
                assetPath.Contains(ToriUtils.CToriGeneratedFolder, StringComparison.OrdinalIgnoreCase) ||
                assetPath.Contains(ToriUtils.CVRCFuryTempFolder, StringComparison.OrdinalIgnoreCase) ||
                assetPath.Contains(ToriUtils.CNDMFGeneratedFolder, StringComparison.OrdinalIgnoreCase)
            )) {
                VRCExpressionsMenu newMenu = ScriptableObject.CreateInstance<VRCExpressionsMenu>();
                EditorUtility.CopySerialized(menu, newMenu);
                newMenu.name = $"{menu.name}{ToriUtils.GeneratedAssetSuffix}";
                assetPath = Path.Combine(ToriUtils.GeneratedAssetFolder, $"{newMenu.name}.asset");
                AssetDatabase.CreateAsset(newMenu, assetPath);
                menu = newMenu;
            }

            _mainMenu = LoadMenu(menu);
            StateHolder.Avatar.expressionsMenu = _mainMenu.Asset;
        }

        private void ProcessControls(SubMenuItem menu, IEnumerable<VRCExpressionsMenu.Control> controls) {
            foreach (var item in controls) {
                switch (item.type) {
                    case ControlType.Button: {
                        menu.Items.Add(new ButtonItem() {
                            Name = item.name,
                            Icon = item.icon,
                            Parameter = item.parameter.name,
                            Value = item.value
                        });
                        break;
                    }
                    case ControlType.Toggle: {
                        menu.Items.Add(new ToggleItem() {
                            Name = item.name,
                            Icon = item.icon,
                            Parameter = item.parameter.name,
                            Value = item.value
                        });
                        break;
                    }
                    case ControlType.SubMenu: {
                        menu.Items.Add(LoadMenu(
                            item.subMenu, item.icon, item.parameter.name, item.value
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

                        menu.Items.Add(new TwoAxisItem() {
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

                        menu.Items.Add(new FourAxisItem() {
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
                        menu.Items.Add(new RadialItem() {
                            Name = item.name,
                            Icon = item.icon,
                            Parameter = item.parameter.name,
                            Value = item.value,
                            RadialParam = param?.name ?? string.Empty
                        });
                        break;
                    }
                    default: {
                        Debug.LogWarning($"[{ToriUtils.LogPrefix}] Unsupported menu item type");
                        break;
                    }
                }
            }
        }

        private SubMenuItem LoadMenu(VRCExpressionsMenu menu, Texture2D icon = null, string parameter = "", float value = 1f) {
            SubMenuItem menuItem = new() {
                Name = menu.name,
                Icon = icon,
                Parameter = parameter,
                Value = value,
                Asset = menu
            };
            ProcessControls(menuItem, menu.controls);
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

                string filePath = ToriUtils.MakeUniqueName(
                    Path.Combine(ToriUtils.GeneratedAssetFolder, $"{item}{ToriUtils.GeneratedAssetSuffix}"),
                    (candidate) => null != AssetDatabase.LoadMainAssetAtPath($"{candidate}.asset")
                ) + ".asset";

                VRCExpressionsMenu menuFile = ScriptableObject.CreateInstance<VRCExpressionsMenu>();
                menuFile.name = Path.GetFileNameWithoutExtension(filePath);
                AssetDatabase.CreateAsset(menuFile, filePath);

                child = new() {
                    Name = item,
                    Asset = menuFile
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

        private SubMenuItem GetOrCreateMenu(SubMenuItem parent, string name, Texture2D icon, string parameter, float value) {
            SubMenuItem menu = parent.GetSubMenu(name);

            if (null != menu)
                return menu;

            string assetPath = ToriUtils.MakeUniqueName(
                Path.Combine(ToriUtils.GeneratedAssetFolder, $"{name}{ToriUtils.GeneratedAssetSuffix}"),
                (candidate) => null != AssetDatabase.LoadMainAssetAtPath($"{candidate}.asset")
            ) + ".asset";

            VRCExpressionsMenu newMenu = ScriptableObject.CreateInstance<VRCExpressionsMenu>();
            newMenu.name = Path.GetFileNameWithoutExtension(assetPath);
            AssetDatabase.CreateAsset(newMenu, assetPath);
            menu = new() {
                Name = name,
                Asset = newMenu,
                Icon = icon,
                Parameter = parameter,
                Value = value
            };
            parent.Items.Add(menu);
            return menu;
        }

        public void AddSubmenu(string menuPrefix, string name, Texture2D icon = null, string parameter = "", float value = 1f) {
            SubMenuItem parent = GetParentMenu(menuPrefix);
            GetOrCreateMenu(parent, name, icon, parameter, value);
        }

        public void AddSubmenu(string menuPrefix, string name, VRCExpressionsMenu menu, Texture2D icon = null, string parameter = "",
            float value = 1f
        ) {
            SubMenuItem parent = GetParentMenu(menuPrefix);
            SubMenuItem menuItem = GetOrCreateMenu(parent, name, icon, parameter, value);
            ProcessControls(menuItem, menu.controls);
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

        public void SetIcon(string itemPath, Texture2D icon) {
            string cleanPath = itemPath.TrimStart('/');
            int lastSlash = cleanPath.LastIndexOf('/');

            string prefix = lastSlash < 0 ? string.Empty : cleanPath[..lastSlash];
            string name = lastSlash < 0 ? cleanPath : cleanPath[(lastSlash + 1)..];

            SubMenuItem parent = GetParentMenu(prefix);
            parent.SetIcon(name, icon);
        }

        public void Build() =>
            ProcessSubMenu(_mainMenu);

        private void ProcessSubMenu(SubMenuItem submenu) {
            if (submenu.Items.Count <= 8) {
                ProcessChunk(submenu.Asset, submenu.Items);
                return;
            }

            int processed = 0;
            int setId = 1;
            VRCExpressionsMenu asset = submenu.Asset;

            while (processed < submenu.Items.Count) {
                int remainder = submenu.Items.Count - processed;
                MenuItem[] chunk;

                if (remainder <= 8) {
                    chunk = new MenuItem[remainder];

                    for (int i = 0; i < remainder; i++) {
                        chunk[i] = submenu.Items[i + processed];
                    }

                    ProcessChunk(asset, chunk);
                    break;
                }

                chunk = new MenuItem[8];

                for (int i = 0; i < 7; i++) {
                    chunk[i] = submenu.Items[i + processed];
                }

                var moreMenu = ScriptableObject.CreateInstance<VRCExpressionsMenu>();
                moreMenu.name = $"{submenu.Asset.name}_{setId++:000}";
                AssetDatabase.CreateAsset(moreMenu, Path.Combine(ToriUtils.GeneratedAssetFolder, $"{moreMenu.name}.asset"));

                chunk[7] = new SubMenuItem() {
                    Name = "More..",
                    Asset = moreMenu,
                    IsMoreMenu = true
                };
                ProcessChunk(asset, chunk);
                asset = moreMenu;
                processed += 7;
            }
        }

        private void ProcessChunk(VRCExpressionsMenu asset, IEnumerable<MenuItem> items) {
            asset.controls.Clear();

            foreach (var item in items) {
                VRCExpressionsMenu.Control control = new() {
                    name = item.Name,
                    icon = item.Icon,
                    parameter = new() { name = item.Parameter },
                    value = item.Value,
                    subParameters = Array.Empty<Parameter>(),
                    labels = Array.Empty<Label>()
                };

                switch (item) {
                    case ButtonItem: {
                        control.type = ControlType.Button;
                        break;
                    }
                    case ToggleItem: {
                        control.type = ControlType.Toggle;
                        break;
                    }
                    case SubMenuItem: {
                        var typedItem = (SubMenuItem)item;
                        control.type = ControlType.SubMenu;
                        control.subMenu = typedItem.Asset;

                        if (!typedItem.IsMoreMenu)
                            ProcessSubMenu(typedItem);

                        break;
                    }
                    case TwoAxisItem: {
                        var typedItem = (TwoAxisItem)item;
                        control.type = ControlType.TwoAxisPuppet;
                        control.subParameters = new Parameter[] {
                            new() { name = typedItem.HorizontalParam },
                            new() { name = typedItem.VerticalParam }
                        };
                        control.labels = new Label[] {
                            new() { name = typedItem.UpLabel,    icon = typedItem.UpIcon    },
                            new() { name = typedItem.RightLabel, icon = typedItem.RightIcon },
                            new() { name = typedItem.DownLabel,  icon = typedItem.DownIcon  },
                            new() { name = typedItem.LeftLabel,  icon = typedItem.LeftIcon  }
                        };
                        break;
                    }
                    case FourAxisItem: {
                        var typedItem = (FourAxisItem)item;
                        control.type = ControlType.FourAxisPuppet;
                        control.subParameters = new Parameter[] {
                            new() { name = typedItem.UpParam },
                            new() { name = typedItem.RightParam },
                            new() { name = typedItem.DownParam },
                            new() { name = typedItem.LeftParam }
                        };
                        control.labels = new Label[] {
                            new() { name = typedItem.UpLabel,    icon = typedItem.UpIcon    },
                            new() { name = typedItem.RightLabel, icon = typedItem.RightIcon },
                            new() { name = typedItem.DownLabel,  icon = typedItem.DownIcon  },
                            new() { name = typedItem.LeftLabel,  icon = typedItem.LeftIcon  }
                        };
                        break;
                    }
                    case RadialItem: {
                        control.type = ControlType.RadialPuppet;
                        control.subParameters = new Parameter[] { new() { name = ((RadialItem)item).RadialParam}};
                        break;
                    }
                    default: continue;
                }

                asset.controls.Add(control);
                ToriUtils.MarkDirty(asset);
            }
        }
    }
}
#endif
