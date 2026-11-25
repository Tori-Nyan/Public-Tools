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
using UnityEditor.Animations;
using UnityEngine;
using VRC.SDK3.Avatars.Components;
using VRC.SDK3.Avatars.ScriptableObjects;

namespace com.torinyan.PubTools.Editor
{
    internal class ParameterInfo
    {
        public string Name;
        public AnimatorControllerParameterType AnimatorType;
        public float DefaultValue;
        public bool AddToVRC;
        public VRCExpressionParameters.ValueType VRCType;
        public bool IsSaved;
        public bool IsSynced;

        private VRCExpressionParameters.ValueType AnimatorTypeToVRC(AnimatorControllerParameterType type) => type switch {
            AnimatorControllerParameterType.Float => VRCExpressionParameters.ValueType.Float,
            AnimatorControllerParameterType.Int => VRCExpressionParameters.ValueType.Int,
            _ => VRCExpressionParameters.ValueType.Bool, // Bool, Trigger, misc
        };

        private void Init(string name, AnimatorControllerParameterType animType, float dfltVal, bool addToVRC,
            VRCExpressionParameters.ValueType vrcType, bool isSaved, bool isSynced) =>
            (Name, AnimatorType, DefaultValue, AddToVRC, VRCType, IsSaved, IsSynced) =
                (name, animType, dfltVal, addToVRC, vrcType, isSaved, isSynced);

        public ParameterInfo(string name, AnimatorControllerParameterType type, float defaultValue, bool addToVRCParams, VRCExpressionParameters.ValueType vrcParamType, bool isSaved, bool isSynced) =>
            Init(name, type, defaultValue, addToVRCParams, vrcParamType, isSaved, isSynced);

        public ParameterInfo(string name, AnimatorControllerParameterType type, float defaultValue, VRCExpressionParameters.ValueType vrcParamType, bool isSaved, bool isSynced) =>
            Init(name, type, defaultValue, true, vrcParamType, isSaved, isSynced);

        public ParameterInfo(string name, AnimatorControllerParameterType type, float defaultValue, bool isSaved, bool isSynced) =>
            Init(name, type, defaultValue, true, AnimatorTypeToVRC(type), isSaved, isSynced);

        public ParameterInfo(string name, AnimatorControllerParameterType type, float defaultValue) =>
            Init(name, type, defaultValue, false, VRCExpressionParameters.ValueType.Float, false, false);

        public ParameterInfo(string name, AnimatorControllerParameterType type) =>
            Init(name, type, 0f, false, VRCExpressionParameters.ValueType.Float, false, false);

        public ParameterInfo(string name) =>
            Init(name, AnimatorControllerParameterType.Float, 0f, false, VRCExpressionParameters.ValueType.Float, false, false);
    }

    internal class AvatarInfo
    {
        public VRCAvatarDescriptor TargetAvatar;
        public bool IsHuman;
        public VRCExpressionParameters VrcParams;
        public VRCExpressionsMenu VrcMenu;

        public AnimatorController GetOrCreateController(VRCAvatarDescriptor.AnimLayerType type) {
            TargetAvatar.customizeAnimationLayers = true;
            FixNullAnimatorCtrlArrays();

            for (int i = 0; i < TargetAvatar.baseAnimationLayers.Length; i++) {
                VRCAvatarDescriptor.CustomAnimLayer item = TargetAvatar.baseAnimationLayers[i];

                if (type != item.type)
                    continue;

                AnimatorController animator = (item.isDefault || null == item.animatorController)
                    ? ToriUtils.GetDefaultController(type)
                    : (item.animatorController as AnimatorController);
                item.animatorController = ToriUtils.CloneAssetIfNotGenerated(animator);
                TargetAvatar.baseAnimationLayers[i] = item;
                return animator;
            }

            for (int i = 0; i < TargetAvatar.specialAnimationLayers.Length; i++) {
                var item = TargetAvatar.specialAnimationLayers[i];

                if (type != item.type)
                    continue;

                AnimatorController animator = (item.isDefault || null == item.animatorController)
                    ? ToriUtils.GetDefaultController(type)
                    : (item.animatorController as AnimatorController);
                item.animatorController = ToriUtils.CloneAssetIfNotGenerated(animator);
                TargetAvatar.specialAnimationLayers[i] = item;
                return animator;
            }

            return null;
        }

        private void FixNullAnimatorCtrlArrays() {
            if (null == TargetAvatar.baseAnimationLayers) {
                TargetAvatar.baseAnimationLayers = IsHuman
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
            }

            if (null == TargetAvatar.specialAnimationLayers) {
                TargetAvatar.specialAnimationLayers = new VRCAvatarDescriptor.CustomAnimLayer[] {
                    new() { type = VRCAvatarDescriptor.AnimLayerType.Sitting, isDefault = true },
                    new() { type = VRCAvatarDescriptor.AnimLayerType.TPose, isDefault = true },
                    new() { type = VRCAvatarDescriptor.AnimLayerType.IKPose, isDefault = true }
                };
            }
        }
    }
}
#endif
