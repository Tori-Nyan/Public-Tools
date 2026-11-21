#if UNITY_EDITOR
using com.torinyan.PubTools.Editor.Features;
using com.torinyan.PubTools.Runtime.Components;
using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using VRC.SDK3.Avatars.Components;
using VRC.SDK3.Avatars.ScriptableObjects;

namespace com.torinyan.PubTools.Editor.Tools
{
    internal class ToriAvatarEditor
    {
        private VRCAvatarDescriptor _avatar;
        private bool _isHuman;

        public void ApplyEdits(VRCAvatarDescriptor avatar, ToriComponent[] components) {
            AssetDatabase.SaveAssets();
            _avatar = avatar;
            _isHuman = avatar.GetComponent<Animator>()?.isHuman ?? false;

            try {
                EnsureOutputFolderExists();

                var oldCustom = _avatar.customExpressions;
                _avatar.customExpressions = true;
                PrepareVrcParams(oldCustom);
                PrepareVrcMenu(oldCustom);
                ToriUtils.MarkDirty(_avatar);

                foreach (var addon in components.Where(x => x is ToriAddonSettings)) {
                    ToriAddon.Process(_avatar, _isHuman, (ToriAddonSettings)addon);
                }

                foreach (var feature in components.Where(x => x is not ToriAddonSettings)) {
                    switch (feature) {
                        case SetMenuIconSettings: {
                            SetMenuIcon.Process(_avatar, (SetMenuIconSettings)feature);
                            break;
                        }
                        default: {
                            Debug.LogWarning($"[{ToriUtils.LogPrefix}] Unsupported feature", feature);
                            break;
                        }
                    }
                }
            } catch(Exception ex) {
                Debug.LogError($"[{ToriUtils.LogPrefix}] Error in `ToriAvatarEditor`");
                Debug.LogException(ex);
            } finally {
                AssetDatabase.SaveAssets();
            }
        }

        private void EnsureOutputFolderExists() {
            if (AssetDatabase.IsValidFolder(ToriUtils.GeneratedAssetFolder))
                return;

            var split = ToriUtils.GeneratedAssetFolder.Split('/');
            AssetDatabase.CreateFolder(split[0], split[1]);
        }

        private void PrepareVrcParams(bool customExpressions) {
            var parameters = _avatar.expressionParameters;

            if (customExpressions && null != parameters) {
                parameters = ToriUtils.CloneAssetIfNotGenerated(parameters);
            } else {
                parameters = ScriptableObject.CreateInstance<VRCExpressionParameters>();
                parameters.parameters = Array.Empty<VRCExpressionParameters.Parameter>();
                AssetDatabase.CreateAsset(parameters, Path.Combine(ToriUtils.GeneratedAssetFolder, "Tori Params.asset"));
            }

            _avatar.expressionParameters = parameters;
        }

        private void PrepareVrcMenu(bool customExpressions) {
            var menu = _avatar.expressionsMenu;

            if (customExpressions && null != menu) {
                menu = ToriUtils.CloneAssetIfNotGenerated(menu);
            } else {
                menu = ScriptableObject.CreateInstance<VRCExpressionsMenu>();
                menu.name = "Tori Menu";
                menu.Parameters = _avatar.expressionParameters;
                AssetDatabase.CreateAsset(menu, Path.Combine(ToriUtils.GeneratedAssetFolder, "Tori Menu.asset"));
            }

            _avatar.expressionsMenu = menu;
        }
    }
}
#endif
