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

            public bool NetworkSynced;

            public byte BitWidth;

            public bool UseCustomLerpDuration;

            public float CustomLerpDuration;

            public bool FlippedRepresentation;
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
            VRCExpressionParameters parameters = avatarParams;
            string assetPath = AssetDatabase.GetAssetPath(parameters);

            if (null == parameters || string.IsNullOrWhiteSpace(assetPath)) {
                parameters = ScriptableObject.CreateInstance<VRCExpressionParameters>();
                parameters.name = "Tori Params";
                parameters.parameters = Array.Empty<VRCExpressionParameters.Parameter>();
                assetPath = Path.Combine(ToriUtils.GeneratedAssetFolder, $"{parameters.name}.asset");
                AssetDatabase.CreateAsset(parameters, assetPath);
            } else if (!(
                assetPath.Contains(ToriUtils.CToriGeneratedFolder, StringComparison.OrdinalIgnoreCase) ||
                assetPath.Contains(ToriUtils.CVRCFuryTempFolder, StringComparison.OrdinalIgnoreCase) ||
                assetPath.Contains(ToriUtils.CNDMFGeneratedFolder, StringComparison.OrdinalIgnoreCase)
            )) {
                parameters = UnityEngine.Object.Instantiate(parameters);
                parameters.name += ToriUtils.GeneratedAssetSuffix;
                assetPath = Path.Combine(ToriUtils.GeneratedAssetFolder, $"{parameters.name}.asset");
                AssetDatabase.CreateAsset(parameters, assetPath);
            }

            foreach (var item in parameters.parameters) {
                _parameters.Add(new() {
                    Name = item.name,
                    Type = item.valueType,
                    Saved = item.saved,
                    DefaultValue = item.defaultValue,
                    NetworkSynced = item.networkSynced,
                    BitWidth = item.bitWidth,
                    UseCustomLerpDuration = item.useCustomLerpDuration,
                    CustomLerpDuration = item.customLerpDuration,
                    FlippedRepresentation = item.flippedRepresentation
                });
            }

            StateHolder.Avatar.expressionParameters = _paramFile;
        }

        public void AddBool(string name, bool val = true, bool saved = true, bool synced = true) =>
            _parameters.Add(new() {
                Name = name,
                Type = VRCExpressionParameters.ValueType.Bool,
                Saved = saved,
                DefaultValue = val ? 1f : 0f,
                NetworkSynced = synced,
                BitWidth = 1
            });

        public void AddInt(string name, byte val = 0, bool saved = true, bool synced = true, byte bitWidth = 8,
            bool customLerp = false, float lerpDuration = 0f, bool flippedRepresentation = false
        ) =>
            _parameters.Add(new() {
                Name = name,
                Type = VRCExpressionParameters.ValueType.Int,
                Saved = saved,
                DefaultValue = val,
                NetworkSynced = synced,
                BitWidth = bitWidth,
                UseCustomLerpDuration = customLerp,
                CustomLerpDuration = lerpDuration,
                FlippedRepresentation = flippedRepresentation
            });

        public void AddFloat(string name, float val = 0f, bool saved = true, bool synced = true, byte bitWidth = 8,
            bool customLerp = false, float lerpDuration = 0f, bool flippedRepresentation = false
        ) =>
            _parameters.Add(new() {
                Name = name,
                Type = VRCExpressionParameters.ValueType.Float,
                Saved = saved,
                DefaultValue = val,
                NetworkSynced = synced,
                BitWidth = bitWidth,
                UseCustomLerpDuration = customLerp,
                CustomLerpDuration = lerpDuration,
                FlippedRepresentation = flippedRepresentation
            });

        public void Build() {
            List<VRCExpressionParameters.Parameter> result = new();

            foreach (var item in _parameters) {
                result.Add(new() {
                    name = item.Name,
                    valueType = item.Type,
                    saved = item.Saved,
                    defaultValue = item.DefaultValue,
                    networkSynced = item.NetworkSynced,
                    bitWidth = item.BitWidth,
                    useCustomLerpDuration = item.UseCustomLerpDuration,
                    customLerpDuration = item.CustomLerpDuration,
                    flippedRepresentation = item.FlippedRepresentation
                });
            }

            _paramFile.parameters = result.ToArray();
        }
    }
}
#endif
