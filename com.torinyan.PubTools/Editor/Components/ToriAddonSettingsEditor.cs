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
    [CustomEditor(typeof(ToriAddonSettings))]
    public class ToriAddonSettingsEditor : ToriSettingsEditor
    {
        protected override GUIContent CEditorTitle => new($"Torinyan Addon {ToriAddonSettings.Version}");

        private SerializedProperty _animatorsProperty;
        private SerializedProperty _parametersProperty;
        private SerializedProperty _menusProperty;

        private SerializedProperty _updateBoundsProperty;
        private SerializedProperty _newBoundsProperty;

        private SerializedProperty _updateAnchorsProperty;
        private SerializedProperty _rootBoneProperty;
        private SerializedProperty _anchorOverrideProperty;

        private SerializedProperty _mergeArmatureProperty;
        private SerializedProperty _mergeSourceProperty;
        private SerializedProperty _mergeTargetProperty;
        private SerializedProperty _mergeAsChildProperty;

        public void OnEnable() {
            _titleStyle = new(EditorStyles.boldLabel);
            _titleStyle.fontSize += 6;
            _editorTitleHeight = GUILayout.Height(_titleStyle.CalcSize(CEditorTitle).y);

            _animatorsProperty = serializedObject.FindProperty("Animators");
            _parametersProperty = serializedObject.FindProperty("Parameters");
            _menusProperty = serializedObject.FindProperty("Menus");

            _updateBoundsProperty = serializedObject.FindProperty("UpdateBounds");
            _newBoundsProperty = serializedObject.FindProperty("NewBounds");

            _updateAnchorsProperty = serializedObject.FindProperty("UpdateAnchors");
            _rootBoneProperty = serializedObject.FindProperty("RootBone");
            _anchorOverrideProperty = serializedObject.FindProperty("AnchorOverride");

            _mergeArmatureProperty = serializedObject.FindProperty("MergeArmature");
            _mergeSourceProperty = serializedObject.FindProperty("MergeSource");
            _mergeTargetProperty = serializedObject.FindProperty("MergeTarget");
            _mergeAsChildProperty = serializedObject.FindProperty("MergeAsChild");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.LabelField(CEditorTitle, _titleStyle, _editorTitleHeight);
            EditorGUILayout.HelpBox(
                "The `Torinyan Addon` component is meant for official Torinyan addons.\n" +
                "These components allow us to automatically apply addons to our avatars, without our end-users having to worry about manually applying them, or relying on external tooling.\n\n" +
                "To be able to test these addons, using the Editor's \"Run\" mode, `VRCFury` or `Modular Avatar` is required in your project.\n" +
                "Without these packages the addons only work on a test build or avatar upload.",
                MessageType.None
            );

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Addon Dependencies", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(_animatorsProperty, new GUIContent("Animators",
                "These animators will be appended to their specified controllers, found on the avatar"
            ), true);
            EditorGUILayout.LabelField(new GUIContent("Parameters",
                "These parameters will be appended to the avatar's VRC expression parameters"
            ));
            EditorGUILayout.PropertyField(_parametersProperty, GUIContent.none, true);
            EditorGUILayout.PropertyField(_menusProperty, new GUIContent("Menus",
                "These menus will be appended at their specified prefix. Missing menus will be created and duplicate menus will be merged."
            ), true);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Mesh Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(_updateBoundsProperty, new GUIContent("Update Bounds",
                "Enables editing the bounds of the addon's meshes"
            ));

            if (_updateBoundsProperty.boolValue) {
                EditorGUILayout.LabelField("Bounds");
                EditorGUILayout.PropertyField(_newBoundsProperty, GUIContent.none, true);
            }

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(_updateAnchorsProperty, new GUIContent("Update Anchors",
                "Enables updating the anchors for the addon's mesh objects"
            ));

            if (_updateAnchorsProperty.boolValue) {
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
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Armature Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(_mergeArmatureProperty, new GUIContent("Merge Armature",
                "Enables merging the addon's armature with the avatar"
            ));

            if (_mergeArmatureProperty.boolValue) {
                EditorGUILayout.LabelField(new GUIContent("Source",
                    "Addon's object to merge to the avatar's Armature"
                ));
                Transform mergeSourceObj = EditorGUILayout.ObjectField(GUIContent.none, null, typeof(Transform), true) as Transform;

                if (null != mergeSourceObj) {
                    string path = GetPath(mergeSourceObj);
                    _mergeSourceProperty.stringValue = path;
                }

                EditorGUILayout.PropertyField(_mergeSourceProperty, GUIContent.none);
                EditorGUILayout.LabelField(new GUIContent("Target",
                    "Path to the avatar's target object, relative to the VRC Avatar Descriptor"
                ));
                Transform mergeTargetObj = EditorGUILayout.ObjectField(GUIContent.none, null, typeof(Transform), true) as Transform;

                if (null != mergeTargetObj) {
                    string path = GetPath(mergeTargetObj);
                    _mergeTargetProperty.stringValue = path;
                }

                EditorGUILayout.PropertyField(_mergeTargetProperty, GUIContent.none);
                EditorGUILayout.PropertyField(_mergeAsChildProperty, new GUIContent("Merge As Child",
                    "When enabled the object will be appended as a child-object. Else it'll be merged"
                ));
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif
