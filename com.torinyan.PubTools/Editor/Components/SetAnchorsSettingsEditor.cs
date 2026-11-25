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
    [CustomEditor(typeof(SetAnchorsSettings)), CanEditMultipleObjects]
    public class SetAnchorsSettingsEditor : ToriSettingsEditor
    {
        protected override GUIContent CEditorTitle => new($"Torinyan Set Anchors {SetAnchorsSettings.Version}");

        private SerializedProperty _rootBoneProperty;
        private SerializedProperty _anchorOverrideProperty;

        public void OnEnable() {
            _titleStyle = new(EditorStyles.boldLabel);
            _titleStyle.fontSize += 6;
            _editorTitleHeight = GUILayout.Height(_titleStyle.CalcSize(CEditorTitle).y);

            _rootBoneProperty = serializedObject.FindProperty("RootBone");
            _anchorOverrideProperty = serializedObject.FindProperty("AnchorOverride");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.LabelField(CEditorTitle, _titleStyle, _editorTitleHeight);
            EditorGUILayout.HelpBox(
                "Torinyan Set Anchors is made for us to be able to set new root-bones and anchor-overrides for child-meshes of this object.",
                MessageType.None
            );

            EditorGUILayout.Space();
            EditorGUILayout.LabelField(new GUIContent("Root Bone",
                "Object path to the new Root Bone for the mesh, leave empty to keep the original."
            ));
            Transform anchorObj = EditorGUILayout.ObjectField(GUIContent.none, null, typeof(Transform), true) as Transform;

            if (null != anchorObj) {
                string path = GetPath(anchorObj);
                _rootBoneProperty.stringValue = path;
            }

            EditorGUILayout.PropertyField(_rootBoneProperty, GUIContent.none);

            EditorGUILayout.LabelField(new GUIContent("Anchor Override",
                "Object path to the new Anchor Override for the mesh, leave empty to keep the original."
            ));
            Transform anchorOverrideObj = EditorGUILayout.ObjectField(GUIContent.none, null, typeof(Transform), true) as Transform;

            if (null != anchorOverrideObj) {
                string path = GetPath(anchorOverrideObj);
                _anchorOverrideProperty.stringValue = path;
            }

            EditorGUILayout.PropertyField(_anchorOverrideProperty, GUIContent.none);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif
