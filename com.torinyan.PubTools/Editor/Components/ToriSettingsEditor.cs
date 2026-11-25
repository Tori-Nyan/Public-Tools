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
using UnityEngine;
using VRC.SDK3.Avatars.Components;

namespace com.torinyan.PubTools.Editor.Components
{
    public abstract class ToriSettingsEditor : UnityEditor.Editor
    {
        protected abstract GUIContent CEditorTitle { get; }

        protected GUIStyle _titleStyle;
        protected GUILayoutOption _editorTitleHeight;

        protected string GetPath(Transform target) {
            Transform self = (serializedObject.targetObject as Component).transform;
            string result = target.name;
            Transform parent = target.parent;

            while (null != parent && self != parent &&
                !parent.TryGetComponent<VRCAvatarDescriptor>(out var _)
            ) {
                result = $"{parent.name}/{result}";
                parent = parent.parent;
            }

            return result;
        }
    }
}
#endif
