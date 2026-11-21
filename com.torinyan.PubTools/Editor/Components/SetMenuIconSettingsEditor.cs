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
    [CustomEditor(typeof(SetMenuIconSettings)), CanEditMultipleObjects]
    public class SetMenuIconSettingsEditor : UnityEditor.Editor
    {
        private readonly static GUIContent CEditorTitle = new($"Torinyan Set Menu Icon {SetMenuIconSettings.Version}");

        private GUIStyle _titleStyle;
        private GUILayoutOption _editorTitleHeight;

        private SerializedProperty _iconProperty;
        private SerializedProperty _itemPathProperty;

        public void OnEnable() {
            _titleStyle = new(EditorStyles.boldLabel);
            _titleStyle.fontSize += 6;
            _editorTitleHeight = GUILayout.Height(_titleStyle.CalcSize(CEditorTitle).y);

            _iconProperty = serializedObject.FindProperty("Icon");
            _itemPathProperty = serializedObject.FindProperty("ItemPath");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.LabelField(CEditorTitle, _titleStyle, _editorTitleHeight);
            EditorGUILayout.HelpBox(
                "Torinyan Set Menu Icon is made for us to be able to set menu icons post-merge of addons.",
                MessageType.None
            );

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(_iconProperty, new GUIContent("Icon", "These new icon to set for the menu item"));
            EditorGUILayout.LabelField(new GUIContent("Item Path", "The menu item path to change the icon for"));
            EditorGUILayout.PropertyField(_itemPathProperty, GUIContent.none, true);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif
