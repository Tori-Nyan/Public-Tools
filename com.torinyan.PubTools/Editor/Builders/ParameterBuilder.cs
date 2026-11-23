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

namespace com.torinyan.PubTools.Editor.Builders
{
    internal class ParameterBuilder
    {
        private class ParameterInfo
        {
            public string Name;
            public VRCExpressionParameters.ValueType Type;

            public bool Saved;

            public float DefaultValue;

            public bool Synced;

#if TORI_VRC_SDK_3_10_6
            public byte BitWidth = 8;

            public bool UseCustomLerpDuration;

            public float CustomLerpDuration;

            public bool FlippedRepresentation;
#endif
        }

        private VRCExpressionParameters _paramFile;
        private readonly List<ParameterInfo> _parameters = new();

        public ParameterBuilder() =>
            Init(null);

        public ParameterBuilder(string assetPath) =>
            Init(AssetDatabase.LoadAssetAtPath<VRCExpressionParameters>(assetPath));

        public ParameterBuilder(VRCExpressionParameters avatarParams) =>
            Init(avatarParams);

        private void Init(VRCExpressionParameters avatarParams) {
            _paramFile = avatarParams;
            string assetPath = null == _paramFile ? string.Empty : AssetDatabase.GetAssetPath(_paramFile);

            if (string.IsNullOrWhiteSpace(assetPath)) {
                _paramFile = ScriptableObject.CreateInstance<VRCExpressionParameters>();
                _paramFile.name = "Tori Params";
                _paramFile.parameters = Array.Empty<VRCExpressionParameters.Parameter>();
                assetPath = Path.Combine(ToriUtils.GeneratedAssetFolder, $"{_paramFile.name}.asset");
                AssetDatabase.CreateAsset(_paramFile, assetPath);
            } else if (!(
                assetPath.Contains(ToriUtils.CToriGeneratedFolder, StringComparison.OrdinalIgnoreCase) ||
                assetPath.Contains(ToriUtils.CVRCFuryTempFolder, StringComparison.OrdinalIgnoreCase) ||
                assetPath.Contains(ToriUtils.CNDMFGeneratedFolder, StringComparison.OrdinalIgnoreCase)
            )) {
                _paramFile = ScriptableObject.CreateInstance<VRCExpressionParameters>();
                EditorUtility.CopySerialized(avatarParams, _paramFile);
                _paramFile.name = $"{avatarParams.name}{ToriUtils.GeneratedAssetSuffix}";
                assetPath = Path.Combine(ToriUtils.GeneratedAssetFolder, $"{_paramFile.name}.asset");
                AssetDatabase.CreateAsset(_paramFile, assetPath);
            }

            AddRawParams(_paramFile.parameters);
            StateHolder.Avatar.expressionParameters = _paramFile;
        }

        public void AddBool(string name, bool val = true, bool saved = true, bool synced = true) =>
            _parameters.Add(new() {
                Name = name,
                Type = VRCExpressionParameters.ValueType.Bool,
                Saved = saved,
                DefaultValue = val ? 1f : 0f,
                Synced = synced
            });

        public void AddInt(string name, byte val = 0, bool saved = true, bool synced = true, byte bitWidth = 8
#if TORI_VRC_SDK_3_10_6
            , bool customLerp = false, float lerpDuration = 0f, bool flippedRepresentation = false
#endif
        ) =>
            _parameters.Add(new() {
                Name = name,
                Type = VRCExpressionParameters.ValueType.Int,
                Saved = saved,
                DefaultValue = val,
                Synced = synced,
#if TORI_VRC_SDK_3_10_6
                BitWidth = bitWidth,
                UseCustomLerpDuration = customLerp,
                CustomLerpDuration = lerpDuration,
                FlippedRepresentation = flippedRepresentation
#endif
            });

        public void AddFloat(string name, float val = 0f, bool saved = true, bool synced = true, byte bitWidth = 8
#if TORI_VRC_SDK_3_10_6
            , bool customLerp = false, float lerpDuration = 0f, bool flippedRepresentation = false
#endif
        ) =>
            _parameters.Add(new() {
                Name = name,
                Type = VRCExpressionParameters.ValueType.Float,
                Saved = saved,
                DefaultValue = val,
                Synced = synced,
#if TORI_VRC_SDK_3_10_6
                BitWidth = bitWidth,
                UseCustomLerpDuration = customLerp,
                CustomLerpDuration = lerpDuration,
                FlippedRepresentation = flippedRepresentation
#endif
            });

        public void AddRawParam(VRCExpressionParameters.Parameter param) =>
            _parameters.Add(new() {
                Name = param.name,
                Type = param.valueType,
                Saved = param.saved,
                DefaultValue = param.defaultValue,
                Synced = param.networkSynced,
#if TORI_VRC_SDK_3_10_6
                BitWidth = param.bitWidth,
                UseCustomLerpDuration = param.useCustomLerpDuration,
                CustomLerpDuration = param.customLerpDuration,
                FlippedRepresentation = param.flippedRepresentation
#endif
            });

        public void AddRawParams(IEnumerable<VRCExpressionParameters.Parameter> parameters) {
            foreach (var item in parameters) {
                /**
                 * Since we don't want to mess with parameters in any way, we skip any parameter
                 * where the name already exists.
                 *
                 * As this is our tooling and will likely only be used by us, this shouldn't be
                 * an issue what so ever. C:
                 */
                if (_parameters.Exists(x => item.name.Equals(x.Name, StringComparison.Ordinal)))
                    continue;

                _parameters.Add(new() {
                    Name = item.name,
                    Type = item.valueType,
                    Saved = item.saved,
                    DefaultValue = item.defaultValue,
                    Synced = item.networkSynced,
#if TORI_VRC_SDK_3_10_6
                    BitWidth = item.bitWidth,
                    UseCustomLerpDuration = item.useCustomLerpDuration,
                    CustomLerpDuration = item.customLerpDuration,
                    FlippedRepresentation = item.flippedRepresentation
#endif
                });
            }
        }

        public void Build() {
            List<VRCExpressionParameters.Parameter> result = new();

            foreach (var item in _parameters) {
                result.Add(new() {
                    name = item.Name,
                    valueType = item.Type,
                    saved = item.Saved,
                    defaultValue = item.DefaultValue,
                    networkSynced = item.Synced,
#if TORI_VRC_SDK_3_10_6
                    bitWidth = item.BitWidth,
                    useCustomLerpDuration = item.UseCustomLerpDuration,
                    customLerpDuration = item.CustomLerpDuration,
                    flippedRepresentation = item.FlippedRepresentation
#endif
                });
            }

            _paramFile.parameters = result.ToArray();
            ToriUtils.MarkDirty(_paramFile);
        }
    }
}
#endif
