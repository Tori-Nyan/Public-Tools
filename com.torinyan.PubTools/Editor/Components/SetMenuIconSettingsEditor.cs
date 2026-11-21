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
