/*
* Torinyan Public Tools
* No license claim here, due to it being based on VRCFury's WhitelistPatch:
* https://github.com/VRCFury/VRCFury/blob/19dcea72e494c7dc6d265259acddda9057ddfd4c/com.vrcfury.vrcfury/Editor/VF/VrcfEditorOnly/WhitelistPatch.cs
*/
#if UNITY_EDITOR
using com.torinyan.PubTools.Runtime.Components;
using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace com.torinyan.PubTools.Editor
{
    internal static class WhitelistPatch
    {
        private readonly static string CToriAddonSettingsName = typeof(ToriAddonSettings).FullName;

        [InitializeOnLoadMethod]
        static void Init() {
            try {
                Debug.Log($"[{ToriUtils.LogPrefix}] Attempting to patch the whitelist in SDKBase ...");
                PatchWhitelist("VRC.SDKBase.Validation.AvatarValidation");
            } catch {
                try {
                    Debug.LogWarning($"[{ToriUtils.LogPrefix}] SDKBase patch failed, falling back to legacy SDK3 patch ...");
                    PatchWhitelist("VRC.SDK3.Validation.AvatarValidation");
                } catch (Exception e) {
                    Debug.LogError(new Exception($"[{ToriUtils.LogPrefix}] Local component whitelist patch failed", e));
                }
            }

            // This is purely here because some other addons initialize the vrcsdk whitelist cache for some reason
            try {
                Debug.Log($"[{ToriUtils.LogPrefix}] Clearing whitelist cache ...");
                FieldInfo cachedWhitelists = GetFieldInfoFromAnyAssembly("VRC.SDKBase.Validation.ValidationUtils", "_whitelistCache");
                object whitelists = cachedWhitelists.GetValue(null);
                whitelists.GetType().GetMethod("Clear").Invoke(whitelists, Array.Empty<object>());
            } catch (Exception e) {
                Debug.LogError(new Exception($"[{ToriUtils.LogPrefix}] Failed to clear whitelist cache", e));
            }
        }

        private static void PatchWhitelist(string type) {
            FieldInfo whitelistField = GetFieldInfoFromAnyAssembly(type, "ComponentTypeWhiteListCommon");
            string[] whitelist = (string[])whitelistField.GetValue(null);
            ArrayUtility.Add(ref whitelist, CToriAddonSettingsName);
            whitelistField.SetValue(null, whitelist);
        }

        private static FieldInfo GetFieldInfoFromAnyAssembly(string type, string field) {
            Type typeObj = AppDomain.CurrentDomain.GetAssemblies()
                .Select(assembly => assembly.GetType(type))
                .FirstOrDefault(t => t != null);
            return typeObj.GetField(field, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
        }
    }
}
#endif
