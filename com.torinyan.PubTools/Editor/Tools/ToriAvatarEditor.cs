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
using com.torinyan.PubTools.Editor.Features;
using com.torinyan.PubTools.Runtime.Components;
using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using VRC.SDK3.Avatars.Components;

namespace com.torinyan.PubTools.Editor.Tools
{
    internal class ToriAvatarEditor
    {
        public void ApplyEdits(VRCAvatarDescriptor avatar, ToriComponent[] components) {
            AssetDatabase.SaveAssets();

            try {
                // Cleanup before build
                if (AssetDatabase.IsValidFolder(ToriUtils.GeneratedAssetFolder)) {
                    var files = Directory.GetFiles(ToriUtils.GeneratedAssetFolder, "*.*", SearchOption.TopDirectoryOnly)
                        .Where(x => !x.EndsWith(".meta"));

                    foreach (var item in files) {
                        AssetDatabase.DeleteAsset(item);
                    }
                }

                ToriUtils.EnsureEmptyAnimExists();
                StateHolder.Load(avatar);

                foreach (var addon in components.Where(x => x is ToriAddonSettings)) {
                    ToriAddon.Process((ToriAddonSettings)addon);
                }

                foreach (var feature in components.Where(x => x is not ToriAddonSettings)) {
                    switch (feature) {
                        case SetMenuIconSettings: {
                            SetMenuIcon.Process((SetMenuIconSettings)feature);
                            break;
                        }
                        default: {
                            Debug.LogWarning($"[{ToriUtils.LogPrefix}] Unsupported feature", feature);
                            break;
                        }
                    }
                }

                StateHolder.Menu.Build();
                StateHolder.Parameter.Build();
            } catch(Exception ex) {
                Debug.LogError($"[{ToriUtils.LogPrefix}] Error in `ToriAvatarEditor`");
                Debug.LogException(ex);
            } finally {
                AssetDatabase.SaveAssets();
                StateHolder.Unload();
            }
        }
    }
}
#endif
