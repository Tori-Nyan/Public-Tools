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
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using VRC.SDK3.Avatars.Components;
using VRC.SDK3.Avatars.ScriptableObjects;

namespace com.torinyan.PubTools.Editor
{
    internal static class ToriUtils
    {
        internal const string CToriGeneratedFolder = "[Torinyan] Generated Assets";
        internal const string CVRCFuryTempFolder = "com.vrcfury.temp";
        internal const string CNDMFGeneratedFolder = "nadena.dev.ndmf";

        internal const string GeneratedAssetFolder = "Assets/" + CToriGeneratedFolder;
        internal const string GeneratedAssetSuffix = "_pesky";

        public const string LogPrefix = "Tori Utils";
        public const string EmptyAnimName = "empty_anim";
        public const string EmptyAnimDirectory = GeneratedAssetFolder + "/anims";
        public const string EmptyAnimPath = EmptyAnimDirectory + "/" + EmptyAnimName + ".anim";
        public readonly static Vector2 AnyStatePos = new(20, -110);
        public readonly static Vector2 ExitStatePos = new(20, -70);
        public readonly static Vector2 EntryStatePos = new(20, -30);
        public readonly static Vector2 IdleStatePos = new(0, 60);

        public static void MarkDirty(UnityEngine.Object obj) {
            EditorUtility.SetDirty(obj);

            if (PrefabUtility.IsPartOfPrefabInstance(obj))
                PrefabUtility.RecordPrefabInstancePropertyModifications(obj);
        }

        public static string MakeUniqueName(string name, Func<string, bool> isUsed) {
            int maxLength = 8;
            name ??= string.Empty;

            for (var i = 0; ; i++) {
                var suffix = i == 0 ? string.Empty : $"_{i + 1}";
                var remaining = maxLength - suffix.Length;

                if (remaining < 0)
                    throw new InvalidOperationException("Unable to generate a unique name.");

                var candidate = name + suffix;

                if (!isUsed(candidate))
                    return candidate;
            }
        }

        public static AnimatorController GetOrCreateController(VRCAvatarDescriptor avatar, bool isHuman, VRCAvatarDescriptor.AnimLayerType type) {
            avatar.customizeAnimationLayers = true;
            FixNullAnimatorCtrlArrays(avatar, isHuman);

            for (int i = 0; i < avatar.baseAnimationLayers.Length; i++) {
                var item = avatar.baseAnimationLayers[i];

                if (type != item.type)
                    continue;

                var animator = (item.isDefault || null == item.animatorController)
                    ? GetDefaultController(type)
                    : (item.animatorController as AnimatorController);
                item.animatorController = CloneAssetIfNotGenerated(animator);
                item.isDefault = false;
                avatar.baseAnimationLayers[i] = item;
                PrefabUtility.RecordPrefabInstancePropertyModifications(avatar);
                return animator;
            }

            for (int i = 0; i < avatar.specialAnimationLayers.Length; i++) {
                var item = avatar.specialAnimationLayers[i];

                if (type != item.type)
                    continue;

                var animator = (item.isDefault || null == item.animatorController)
                    ? GetDefaultController(type)
                    : (item.animatorController as AnimatorController);
                item.animatorController = CloneAssetIfNotGenerated(animator);
                item.isDefault = false;
                avatar.specialAnimationLayers[i] = item;
                PrefabUtility.RecordPrefabInstancePropertyModifications(avatar);
                return animator;
            }

            return null;
        }

        private static void FixNullAnimatorCtrlArrays(VRCAvatarDescriptor avatar, bool isHuman) {
            if (null == avatar.baseAnimationLayers) {
                avatar.baseAnimationLayers = isHuman
                    ? new VRCAvatarDescriptor.CustomAnimLayer[] {
                        new() { type = VRCAvatarDescriptor.AnimLayerType.Base, isDefault = true },
                        new() { type = VRCAvatarDescriptor.AnimLayerType.Additive, isDefault = true },
                        new() { type = VRCAvatarDescriptor.AnimLayerType.Gesture, isDefault = true },
                        new() { type = VRCAvatarDescriptor.AnimLayerType.Action, isDefault = true },
                        new() { type = VRCAvatarDescriptor.AnimLayerType.FX, isDefault = true }
                    }
                    : new VRCAvatarDescriptor.CustomAnimLayer[] {
                        new() { type = VRCAvatarDescriptor.AnimLayerType.Base, isDefault = true },
                        new() { type = VRCAvatarDescriptor.AnimLayerType.Action, isDefault = true },
                        new() { type = VRCAvatarDescriptor.AnimLayerType.FX, isDefault = true }
                    };
                PrefabUtility.RecordPrefabInstancePropertyModifications(avatar);
            }

            if (null == avatar.specialAnimationLayers) {
                avatar.specialAnimationLayers = new VRCAvatarDescriptor.CustomAnimLayer[] {
                    new() { type = VRCAvatarDescriptor.AnimLayerType.Sitting, isDefault = true },
                    new() { type = VRCAvatarDescriptor.AnimLayerType.TPose, isDefault = true },
                    new() { type = VRCAvatarDescriptor.AnimLayerType.IKPose, isDefault = true }
                };
                PrefabUtility.RecordPrefabInstancePropertyModifications(avatar);
            }
        }

        public static AnimatorController GetDefaultController(VRCAvatarDescriptor.AnimLayerType type) {
            string guid;

            switch (type) {
                case VRCAvatarDescriptor.AnimLayerType.Base: {
                    // vrc_AvatarV3LocomotionLayer
                    guid = "4e4e1a372a526074884b7311d6fc686b";
                    break;
                }
                case VRCAvatarDescriptor.AnimLayerType.Additive: {
                    // vrc_AvatarV3IdleLayer
                    guid = "573a1373059632b4d820876efe2d277f";
                    break;
                }
                case VRCAvatarDescriptor.AnimLayerType.Gesture: {
                    // vrc_AvatarV3HandsLayer2
                    guid = "5ecf8b95a27552840aef66909bdf588f";
                    break;
                }
                case VRCAvatarDescriptor.AnimLayerType.Action: {
                    // vrc_AvatarV3ActionLayer
                    guid = "3e479eeb9db24704a828bffb15406520";
                    break;
                }
                case VRCAvatarDescriptor.AnimLayerType.Sitting: {
                    // vrc_AvatarV3SittingLayer2
                    guid = "74c2e15937e5c95478edd251f20e126f";
                    break;
                }
                case VRCAvatarDescriptor.AnimLayerType.TPose: {
                    // vrc_AvatarV3UtilityTPose
                    guid = "00121b5812372b74f9012473856d8acf";
                    break;
                }
                case VRCAvatarDescriptor.AnimLayerType.IKPose: {
                    // vrc_AvatarV3UtilityIKPose
                    guid = "a9b90a833b3486e4b82834c9d1f7c4ee";
                    break;
                }
                default: {
                    // VRCAvatarDescriptor.AnimLayerType.FX
                    AnimatorController fxCtrl = new() { name = "FX_Tori" };
                    AssetDatabase.CreateAsset(fxCtrl, Path.Combine(GeneratedAssetFolder, $"{fxCtrl.name}.controller"));

                    fxCtrl.parameters = new AnimatorControllerParameter[] {
                        new() {
                            name = "Blend",
                            type = AnimatorControllerParameterType.Float,
                            defaultFloat = 1f
                        }
                    };

                    var baseLayer = CreateLayer(fxCtrl, "Base Layer");
                    baseLayer.stateMachine.AddStateMachine("hehe, torinyan was here\nDon't you dare clone our stuff\nI WILL kick peddles in your general direction if you do", new(0, -240));
                    return fxCtrl;
                }
            }

            var path = AssetDatabase.GUIDToAssetPath(guid) ??
                throw new Exception($"[{LogPrefix}] Failed to find default {type} controller");

            var c = AssetDatabase.LoadAssetAtPath<AnimatorController>(path) ??
                throw new Exception($"[{LogPrefix}] Failed to load default {type} controller");

            return c;
        }

        internal static T CloneAssetIfNotGenerated<T>(T asset) where T : UnityEngine.Object {
            var assetPath = AssetDatabase.GetAssetPath(asset);

            if (!string.IsNullOrWhiteSpace(assetPath) && (
                assetPath.Contains(CToriGeneratedFolder, StringComparison.OrdinalIgnoreCase) ||
                assetPath.Contains(CVRCFuryTempFolder, StringComparison.OrdinalIgnoreCase) ||
                assetPath.Contains(CNDMFGeneratedFolder, StringComparison.OrdinalIgnoreCase)
            ))
                return asset;

            string ext;

            if (asset is AnimationClip) {
                ext = "anim";
            } else if (asset is Material) {
                ext = "mat";
            } else if (asset is AnimatorController) {
                ext = "controller";
            } else if (asset is AvatarMask) {
                ext = "mask";
            } else {
                ext = "asset";
            }

            var newAssetPath = Path.Combine(GeneratedAssetFolder, $"{asset.name}{GeneratedAssetSuffix}.{ext}");
            AssetDatabase.CopyAsset(assetPath, newAssetPath);
            return AssetDatabase.LoadAssetAtPath<T>(newAssetPath);
        }

        public static byte GetParameterCost(VRCExpressionParameters.Parameter parameter) {
#if TORI_VRC_SDK_3_10_6
            return parameter.GetBitWidthByType();
#else
            return (byte)VRCExpressionParameters.TypeCost(parameter.valueType);
#endif
        }

        public static bool GetAnimatorWDPreference(AnimatorController animator, StringBuilder debug = null) {
            var layersWithStates = animator.layers.Where(x => x.stateMachine.states.Length > 0);
            var onStates = layersWithStates.Sum(x =>
                x.stateMachine.states.Count(y => y.state.writeDefaultValues)
            );
            var offStates = layersWithStates.Sum(x =>
                x.stateMachine.states.Count(y => !y.state.writeDefaultValues)
            );
            var makeStatesWD = onStates > offStates;

            debug?.AppendLine($"\nWD decision ({onStates} on | {offStates} off) = {makeStatesWD}");
            return makeStatesWD;
        }

        public static AnimationClip EnsureEmptyAnimExists() {
            var result = AssetDatabase.LoadAssetAtPath<AnimationClip>(EmptyAnimPath);

            if (null != result)
                return result;

            var split = EmptyAnimDirectory.Split('/');
            var curPath = split[0];

            for (int i = 1; i < split.Length; i++) {
                var folder = split[i];

                if (!AssetDatabase.IsValidFolder($"{curPath}/{folder}"))
                    AssetDatabase.CreateFolder(curPath, folder);

                curPath += $"/{folder}";
            }

            result = new() {
                name = EmptyAnimName,
                hideFlags = HideFlags.None,
                legacy = false
            };
            AssetDatabase.CreateAsset(result, EmptyAnimPath);

            var settings = AnimationUtility.GetAnimationClipSettings(result);
            settings.loopTime = false;
            AnimationUtility.SetAnimationClipSettings(result, settings);

            result.SetCurve(
                "_ignored", typeof(GameObject), "m_IsActive",
                AnimationCurve.Constant(0f, 1f / result.frameRate, 0f)
            );
            return result;
        }

        public static void CreateParameter(AnimatorController animator, ParameterInfo info, VRCExpressionParameters vrcParameters = null) {
            if (!animator.parameters.Any(x => x.name.Equals(info.Name, StringComparison.Ordinal))) {
                var animArray = animator.parameters;
                ArrayUtility.Add(ref animArray, new() {
                    name = info.Name,
                    type = info.AnimatorType,
                    defaultFloat = info.DefaultValue,
                    defaultInt = Mathf.RoundToInt(info.DefaultValue),
                    defaultBool = info.DefaultValue > 0f
                });
                animator.parameters = animArray;
            }

            if (null != vrcParameters && info.AddToVRC && !vrcParameters.parameters.Any(x => x.name == info.Name)) {
                var vrcArray = vrcParameters.parameters;
                ArrayUtility.Add(ref vrcArray, new() {
                    name = info.Name,
                    valueType = info.VRCType,
                    defaultValue = info.DefaultValue,
                    networkSynced = info.IsSynced,
                    saved = info.IsSaved
                });
                vrcParameters.parameters = vrcArray;
                EditorUtility.SetDirty(vrcParameters);
            }
        }

        public static VRCExpressionsMenu GetMenuForItem(VRCExpressionsMenu mainMenu, string menuFolder, string menuItemPath) {
            if (null == mainMenu || string.IsNullOrWhiteSpace(menuFolder) || string.IsNullOrWhiteSpace(menuItemPath))
                return null;

            var pathSplit = menuItemPath.Trim().Split('/');

            if (pathSplit.Length <= 1)
                return mainMenu;

            VRCExpressionsMenu parentMenu = null;
            VRCExpressionsMenu submenu = null;

            for (int i = 0; i < pathSplit.Length - 1; i++) {
                var nameStr = pathSplit[i];
                var submenuName = $"{mainMenu.name} {nameStr}";
                var submenuPath = $"{menuFolder}/{submenuName}.asset";
                submenu = AssetDatabase.LoadAssetAtPath<VRCExpressionsMenu>(submenuPath);

                if (null == submenu) {
                    submenu = ScriptableObject.CreateInstance<VRCExpressionsMenu>();
                    submenu.name = submenuName;
                    submenu.Parameters = mainMenu.Parameters;
                    AssetDatabase.CreateAsset(submenu, submenuPath);

                    (parentMenu ?? mainMenu).controls.Add(new() {
                        name = nameStr,
                        type = VRCExpressionsMenu.Control.ControlType.SubMenu,
                        subMenu = submenu,
                        parameter = new() { name = string.Empty },
                        subParameters = Array.Empty<VRCExpressionsMenu.Control.Parameter>()
                    });
                }

                parentMenu = submenu;
            }

            return submenu;
        }

        public static AnimatorControllerLayer CreateLayer(AnimatorController animator, string layerName) {
            var cleanName = animator.MakeUniqueLayerName(layerName.Trim());
            AnimatorControllerLayer newLayer = new() {
                name = cleanName,
                blendingMode = AnimatorLayerBlendingMode.Override,
                defaultWeight = 1,
                stateMachine = new() {
                    hideFlags = HideFlags.HideInHierarchy,
                    name = cleanName,
                    anyStatePosition = AnyStatePos,
                    exitPosition = ExitStatePos,
                    entryPosition = EntryStatePos
                }
            };
            AssetDatabase.AddObjectToAsset(newLayer.stateMachine, AssetDatabase.GetAssetPath(animator));

            var array = animator.layers;
            ArrayUtility.Add(ref array, newLayer);
            animator.layers = array;

            return newLayer;
        }

        public static AnimatorState CreateState(AnimatorStateMachine stateMachine, string stateName, Vector3 position, bool isWD, Motion motion = null, bool isDefault = false) {
            AnimatorState newState = new() {
                hideFlags = HideFlags.HideInHierarchy,
                name = stateName,
                behaviours = Array.Empty<StateMachineBehaviour>(),
                writeDefaultValues = isWD,
                motion = motion
            };
            AssetDatabase.AddObjectToAsset(newState, AssetDatabase.GetAssetPath(stateMachine));

            var array = stateMachine.states;
            ArrayUtility.Add(ref array, new() {
                state = newState,
                position = position
            });
            stateMachine.states = array;

            if (isDefault)
                stateMachine.defaultState = newState;

            return newState;
        }

        public static AnimatorStateTransition CreateTransition(AnimatorState srcState, AnimatorState dstState, bool hasExitTime = true) {
            AnimatorStateTransition newTrans = new() {
                hideFlags = HideFlags.HideInHierarchy,
                destinationState = dstState,
                hasExitTime = hasExitTime,
                exitTime = 0f,
                hasFixedDuration = true,
                duration = 0f,
                offset = 0f,
                canTransitionToSelf = false,
                conditions = Array.Empty<AnimatorCondition>()
            };
            AssetDatabase.AddObjectToAsset(newTrans, AssetDatabase.GetAssetPath(srcState));

            var array = srcState.transitions;
            ArrayUtility.Add(ref array, newTrans);
            srcState.transitions = array;

            return newTrans;
        }

        public static void AddCondition(AnimatorStateTransition transition, string name, AnimatorConditionMode mode, float threshold = 0f) {
            var array = transition.conditions;
            ArrayUtility.Add(ref array, new() {
                parameter = name,
                mode = mode,
                threshold = threshold
            });
            transition.conditions = array;
        }
    }
}
#endif
