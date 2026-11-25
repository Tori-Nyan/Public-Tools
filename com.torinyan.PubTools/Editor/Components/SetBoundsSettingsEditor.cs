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
using com.torinyan.PubTools.Runtime.Components;
using UnityEditor;
using UnityEngine;

namespace com.torinyan.PubTools.Editor.Components
{
    [CustomEditor(typeof(SetBoundsSettings)), CanEditMultipleObjects]
    public class SetBoundsSettingsEditor : ToriSettingsEditor
    {
        protected override GUIContent CEditorTitle => new($"Torinyan Set Bounds {SetBoundsSettings.Version}");

        private SerializedProperty _newBoundsProperty;

        public void OnEnable() {
            _titleStyle = new(EditorStyles.boldLabel);
            _titleStyle.fontSize += 6;
            _editorTitleHeight = GUILayout.Height(_titleStyle.CalcSize(CEditorTitle).y);

            _newBoundsProperty = serializedObject.FindProperty("NewBounds");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.LabelField(CEditorTitle, _titleStyle, _editorTitleHeight);
            EditorGUILayout.HelpBox(
                "Torinyan Set Bounds is made for us to be able to set new bounds for child-meshes of this object.",
                MessageType.None
            );

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Bounds");
            EditorGUILayout.PropertyField(_newBoundsProperty, GUIContent.none, true);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif
