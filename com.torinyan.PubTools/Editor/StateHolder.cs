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
using UnityEditor;
using UnityEngine;

namespace com.torinyan.PubTools.Editor
{
    internal static class StateHolder
    {
        private static readonly List<Transform> _objStates = new();

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
    }
}
#endif
