#if UNITY_EDITOR
using com.torinyan.PubTools.Runtime;
using com.torinyan.PubTools.Runtime.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VRC.SDK3.Avatars.Components;
using VRC.SDK3.Avatars.ScriptableObjects;

namespace com.torinyan.PubTools.Editor.Features
{
    internal static class ToriAddon
    {
        public static void Process(VRCAvatarDescriptor avatar, bool isHuman, ToriAddonSettings settings) {
            var skinnedMeshes = settings.GetComponentsInChildren<SkinnedMeshRenderer>();
            var staticMeshes = settings.GetComponentsInChildren<MeshRenderer>();

            MergeParameters(avatar, settings.Parameters.parameters);

            foreach (var menu in settings.Menus) {
                MergeMenu(avatar, menu);
            }

            var mergeTarget = string.Empty;

            if (settings.MergeArmature) {
                // We should probably check and create empties if target is missing
                var objectName = settings.MergeSource[(settings.MergeSource.LastIndexOf('/') + 1)..];
                mergeTarget = $"{settings.MergeTarget}/{objectName}";

                if (settings.MergeAsChild) {
                    mergeTarget = ToriUtils.MakeUniqueName(
                        mergeTarget,
                        (candidate) => null != avatar.transform.Find(candidate)
                    );
                    objectName = mergeTarget[(mergeTarget.LastIndexOf('/') + 1)..];
                }

                MergeArmature(avatar, settings.transform.Find(settings.MergeSource), settings.MergeTarget, objectName, settings.MergeAsChild);
            }

            if (settings.UpdateAnchors)
                UpdateAnchors(avatar, skinnedMeshes, staticMeshes, settings.Anchor, settings.AnchorOverride);

            if (settings.UpdateBounds)
                UpdateBounds(skinnedMeshes, settings.NewBounds);

            foreach (var animator in settings.Animators) {
                MergeAnimator(avatar, isHuman, animator, settings.MergeSource, mergeTarget);
            }
        }

        private static void MergeParameters(VRCAvatarDescriptor avatar, VRCExpressionParameters.Parameter[] parameters) {
            List<VRCExpressionParameters.Parameter> paramArr = new(avatar.expressionParameters.parameters);

            foreach (var parameter in parameters) {
                if (null != avatar.expressionParameters.FindParameter(parameter.name))
                    continue;

                paramArr.Add(new() {
                    name = parameter.name,
                    valueType = parameter.valueType,
                    defaultValue = parameter.defaultValue,
#if TORI_VRC_SDK_3_10_6
                    bitWidth = parameter.bitWidth,
                    useCustomLerpDuration = parameter.useCustomLerpDuration,
                    flippedRepresentation = parameter.flippedRepresentation,
#endif
                    saved = parameter.saved,
                    networkSynced = parameter.networkSynced
                });
            }

            avatar.expressionParameters.parameters = paramArr.ToArray();
        }

        private static void MergeMenu(VRCAvatarDescriptor avatar, AddonMenuInfo menu) {
            var itemName = menu.Prefix[(menu.Prefix.LastIndexOf('/') + 1)..];
            var parent = ToriUtils.GetMenuForItem(avatar.expressionsMenu, ToriUtils.GeneratedAssetFolder, menu.Prefix);

            foreach (var item in parent.controls.Where(x => VRCExpressionsMenu.Control.ControlType.SubMenu == x.type)) {
                if (item.name.Equals(itemName, StringComparison.Ordinal)) {
                    // Merge menu items, make unique name if button exists of different type or parameter, do the more.. stuff
                    return;
                }
            }

            parent.controls.Add(new() {
                name = ToriUtils.MakeUniqueName(itemName, (candidate) => parent.controls.Any(x =>
                    x.name.Equals(candidate, StringComparison.Ordinal)
                )),
                type = VRCExpressionsMenu.Control.ControlType.SubMenu,
                subMenu = menu.Menu,
                parameter = new() { name = string.Empty },
                subParameters = Array.Empty<VRCExpressionsMenu.Control.Parameter>()
            });
        }

        private static void UpdateAnchors(VRCAvatarDescriptor avatar, SkinnedMeshRenderer[] skinnedMeshes, MeshRenderer[] staticMeshes,
            string rootBone, string anchorOverride
        ) {
            foreach (var mesh in skinnedMeshes) {
                if (!string.IsNullOrWhiteSpace(rootBone)) {
                    var newRoot = avatar.transform.Find(rootBone);

                    if (null != newRoot)
                        mesh.rootBone = newRoot;
                }

                if (!string.IsNullOrWhiteSpace(anchorOverride)) {
                    var newAnchor = avatar.transform.Find(anchorOverride);

                    if (null != newAnchor)
                        mesh.probeAnchor = newAnchor;
                }

                ToriUtils.MarkDirty(mesh);
            }

            foreach (var mesh in staticMeshes) {
                if (!string.IsNullOrWhiteSpace(anchorOverride)) {
                    var newAnchor = avatar.transform.Find(anchorOverride);

                    if (null != newAnchor)
                        mesh.probeAnchor = newAnchor;

                    ToriUtils.MarkDirty(mesh);
                }
            }
        }

        private static void UpdateBounds(SkinnedMeshRenderer[] skinnedMeshes, Bounds bounds) {
            foreach (var mesh in skinnedMeshes) {
                mesh.localBounds = bounds;
                ToriUtils.MarkDirty(mesh);
            }
        }

        private static void MergeArmature(VRCAvatarDescriptor avatar, Transform sourceObj, string parent, string name, bool asChild) {
            // we have to prefix every animation property as well.. ._."
            Transform parentObj = EnsureTargetExists(avatar.transform, parent);

            if (null == parentObj)
                throw new InvalidOperationException($"Unable to find or create target oject at path `{parent}`");

            if (asChild) {
                sourceObj.name = name;
                sourceObj.SetParent(parentObj);
                ToriUtils.MarkDirty(sourceObj);
                return;
            }

            /* Make magic merge code, copy components when object exists, etc
            parentObj = EnsureTargetExists(avatar.transform, target);

            if (null == parentObj)
                throw new InvalidOperationException($"Unable to find or create target oject at path `{target}`");

            while (sourceObj.childCount > 0) {
                var child = sourceObj.GetChild(0);
                child.SetParent(parentObj);
                ToriUtils.MarkDirty(child);
            }
            */
        }

        private static Transform EnsureTargetExists(Transform root, string target) {
            var parentObj = root.Find(target);

            if (null == parentObj) {
                List<string> missingParents = new() {
                    target[(target.LastIndexOf('/') + 1)..]
                };
                var candidate = target[..target.LastIndexOf('/')];
                Transform candidateObj = root.Find(candidate);

                while (null == candidateObj && candidate.IndexOf('/') > 0) {
                    missingParents.Add(candidate[(candidate.LastIndexOf('/') + 1)..]);
                    candidate = candidate[..candidate.LastIndexOf('/')];
                    candidateObj = root.Find(candidate);
                }

                if (null == candidateObj)
                    return null; // Failed to find anything valid

                parentObj = candidateObj;

                if (missingParents.Count == 0)
                    return parentObj;

                for (int i = missingParents.Count - 1; i >= 0; i--) {
                    GameObject newGo = new(missingParents[i]);
                    newGo.transform.SetParent(parentObj);
                    parentObj = newGo.transform;
                }

                ToriUtils.MarkDirty(candidateObj);
            }

            return parentObj;
        }

        private static void MergeAnimator(VRCAvatarDescriptor avatar, bool isHuman, AddonAnimatorInfo animator, string oldAnimPath,
            string newAnimPath
        ) {
            var srcCtrl = animator.Controller;
            var dstCrtl = ToriUtils.GetOrCreateController(avatar, isHuman, animator.Type);

            foreach (var clip in srcCtrl.animationClips) {
                // Clone these into a single asset file
            }

            // Do fancy merge stuff
        }
    }
}
#endif
