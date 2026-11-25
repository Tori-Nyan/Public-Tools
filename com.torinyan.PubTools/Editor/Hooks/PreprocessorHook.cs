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
using com.torinyan.PubTools.Editor.Tools;
using com.torinyan.PubTools.Runtime.Components;
using UnityEngine;
using VRC.SDK3.Avatars.Components;
using VRC.SDKBase.Editor.BuildPipeline;

namespace com.torinyan.PubTools.Editor.Hooks
{
    internal class PreprocessorHook : IVRCSDKPreprocessAvatarCallback
    {
        // Must run before VRCFury (-10000) and Modular Avatar (-11000)
        public int callbackOrder => -12000;

        public bool OnPreprocessAvatar(GameObject obj) {
            if (!StateHolder.ShouldProcess(obj)) {
                Debug.LogWarning($"[{ToriUtils.LogPrefix}] Skipping `PreprocessorHook` because preprocessors already ran on this object", obj);
                return true;
            }

            if (!obj.TryGetComponent<VRCAvatarDescriptor>(out var avatar)) {
                Debug.LogWarning($"[{ToriUtils.LogPrefix}] Skipping `PreprocessorHook` because `VRCAvatarDescriptor` component could not be found", obj);
                return true;
            }

            ToriComponent[] components = obj.GetComponentsInChildren<ToriComponent>();

            if (null == components || components.Length <= 0) {
                Debug.Log($"[{ToriUtils.LogPrefix}] Skipping `PreprocessorHook` because there is nothing to process", obj);
                return true;
            }

            try {
                ToriAvatarEditor editor = new();
                editor.ApplyEdits(avatar, components);

                foreach (var item in components) {
                    Object.DestroyImmediate(item);
                }

                return true;
            } catch (System.Exception ex) {
                Debug.LogError($"[{ToriUtils.LogPrefix}] Error in `PreprocessorHook`", obj);
                Debug.LogException(ex, obj);

                return false;
            }
        }
    }
}
#endif
