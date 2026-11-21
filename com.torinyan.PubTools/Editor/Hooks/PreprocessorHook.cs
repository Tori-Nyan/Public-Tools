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

            var components = obj.GetComponentsInChildren<ToriComponent>();

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
