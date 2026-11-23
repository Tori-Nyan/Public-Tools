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
using com.torinyan.PubTools.Runtime;
using com.torinyan.PubTools.Runtime.Components;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace com.torinyan.PubTools.Editor.Features
{
    internal static class ToriAddon
    {
        public static void Process(ToriAddonSettings settings) {
            var skinnedMeshes = settings.GetComponentsInChildren<SkinnedMeshRenderer>();
            var staticMeshes = settings.GetComponentsInChildren<MeshRenderer>();

            StateHolder.Parameter.AddRawParams(settings.Parameters.parameters);

            foreach (var menu in settings.Menus) {
                string cleanPrefix = menu.Prefix.TrimStart('/');
                int lastSlash = cleanPrefix.LastIndexOf('/');

                string prefix = lastSlash < 0 ? string.Empty : cleanPrefix[..lastSlash];
                string name = lastSlash < 0 ? cleanPrefix : cleanPrefix[(lastSlash + 1)..];
                StateHolder.Menu.AddSubmenu(prefix, name, menu.Menu, menu.Icon, menu.Parameter, menu.Value);
            }

            var mergeTarget = string.Empty;

            if (settings.MergeArmature) {
                // We should probably check and create empties if target is missing
                var objectName = settings.MergeSource[(settings.MergeSource.LastIndexOf('/') + 1)..];
                mergeTarget = $"{settings.MergeTarget}/{objectName}";

                if (settings.MergeAsChild) {
                    mergeTarget = ToriUtils.MakeUniqueName(
                        mergeTarget,
                        (candidate) => null != StateHolder.Avatar.transform.Find(candidate)
                    );
                    objectName = mergeTarget[(mergeTarget.LastIndexOf('/') + 1)..];
                }

                MergeArmature(settings.transform.Find(settings.MergeSource), settings.MergeTarget, objectName, settings.MergeAsChild);
            }

            if (settings.UpdateAnchors)
                UpdateAnchors(skinnedMeshes, staticMeshes, settings.Anchor, settings.AnchorOverride);

            if (settings.UpdateBounds)
                UpdateBounds(skinnedMeshes, settings.NewBounds);

            foreach (var animator in settings.Animators) {
                MergeAnimator(animator, settings.MergeSource, mergeTarget);
            }
        }

        private static void UpdateAnchors(SkinnedMeshRenderer[] skinnedMeshes, MeshRenderer[] staticMeshes,
            string rootBone, string anchorOverride
        ) {
            foreach (var mesh in skinnedMeshes) {
                if (!string.IsNullOrWhiteSpace(rootBone)) {
                    var newRoot = StateHolder.Avatar.transform.Find(rootBone);

                    if (null != newRoot)
                        mesh.rootBone = newRoot;
                }

                if (!string.IsNullOrWhiteSpace(anchorOverride)) {
                    var newAnchor = StateHolder.Avatar.transform.Find(anchorOverride);

                    if (null != newAnchor)
                        mesh.probeAnchor = newAnchor;
                }

                ToriUtils.MarkDirty(mesh);
            }

            foreach (var mesh in staticMeshes) {
                if (!string.IsNullOrWhiteSpace(anchorOverride)) {
                    var newAnchor = StateHolder.Avatar.transform.Find(anchorOverride);

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

        private static void MergeArmature(Transform sourceObj, string parent, string name, bool asChild) {
            // we have to prefix every animation property as well.. ._."
            Transform parentObj = EnsureTargetExists(StateHolder.Avatar.transform, parent);

            if (null == parentObj)
                throw new InvalidOperationException($"Unable to find or create target oject at path `{parent}`");

            if (asChild) {
                sourceObj.name = name;
                sourceObj.SetParent(parentObj);
                ToriUtils.MarkDirty(sourceObj);
                return;
            }

            /* Make magic merge code, copy components when object exists, etc
            parentObj = EnsureTargetExists(StateHolder.Avatar.transform, target);

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

        private static void MergeAnimator(AddonAnimatorInfo animator, string oldAnimPath,
            string newAnimPath
        ) {
            var srcCtrl = animator.Controller;
            var dstCrtl = ToriUtils.GetOrCreateController(StateHolder.Avatar, StateHolder.IsHuman, animator.Type);

            foreach (var clip in srcCtrl.animationClips) {
                // Clone these into a single asset file
            }

            // Do fancy merge stuff
        }
    }
}
#endif
