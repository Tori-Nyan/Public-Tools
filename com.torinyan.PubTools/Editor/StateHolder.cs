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
using System.Collections.Generic;
using com.torinyan.PubTools.Editor.Builders;
using UnityEditor;
using UnityEngine;
using VRC.SDK3.Avatars.Components;

namespace com.torinyan.PubTools.Editor
{
    internal static class StateHolder
    {
        private static readonly List<Transform> _objStates = new();

        public static VRCAvatarDescriptor Avatar { get; private set; }
        public static bool IsHuman { get; private set; }
        public static MenuBuilder Menu { get; private set; }
        public static ParameterBuilder Parameter { get; private set; }

        [InitializeOnLoadMethod]
        static void Init() {
            EditorApplication.playModeStateChanged += (state) => {
                if (state == PlayModeStateChange.ExitingPlayMode)
                    _objStates.Clear();
            };
        }

        public static bool ShouldProcess(GameObject go) =>
            !Application.isPlaying || !_objStates.Contains(go.transform);

        public static void SetProcessed(GameObject go) {
            var goTrans = go.transform;

            if (!_objStates.Contains(goTrans))
                _objStates.Add(goTrans);
        }

        public static void Load(VRCAvatarDescriptor avatar) {
            Avatar = avatar;
            IsHuman = Avatar.GetComponent<Animator>()?.isHuman ?? false;

            Avatar.customExpressions = true;

            Menu = Avatar.customExpressions ? new(Avatar.expressionsMenu) : new();
            Parameter = Avatar.customExpressions ? new(Avatar.expressionParameters) : new();

            ToriUtils.MarkDirty(Avatar);
        }

        public static void Unload() {
            Parameter = null;
            Menu = null;
            Avatar = null;
        }
    }
}
#endif
