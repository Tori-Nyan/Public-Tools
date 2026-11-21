#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace com.torinyan.PubTools.Editor.Tools
{
    public class CreateNiceLayer : EditorWindow
    {
        private const string CVersion = "v1.0.0";
        private static Vector2 CWindowSize = new(420, 86);

        private AnimatorController _animator;
        private string _layerName = string.Empty;

        [MenuItem("Tools/Torinyan/Create Nice Layer", priority = 1)]
        public static void ShowWindow() {
            var wnd = GetWindow<CreateNiceLayer>(true, $"[Torinyan] Create Nice Layer {CVersion}", true);
            wnd.minSize = CWindowSize;
            wnd.maxSize = CWindowSize;
        }

        void OnGUI()
        {
            EditorGUILayout.Space();
            _animator = (AnimatorController)EditorGUILayout.ObjectField("Target Animator", _animator, typeof(AnimatorController), true);

            EditorGUILayout.Space();
            _layerName = EditorGUILayout.TextField("Layer Name", _layerName);

            EditorGUILayout.Space();
            EditorGUI.BeginDisabledGroup(null == _animator || string.IsNullOrWhiteSpace(_layerName));
            {
                if (GUILayout.Button("Create"))
                    CreateLayer();
            }
            EditorGUI.EndDisabledGroup();
        }

        private void CreateLayer() {
            try {
                AssetDatabase.StartAssetEditing();

                var makeStatesWD = ToriUtils.GetAnimatorWDPreference(_animator);
                var _emptyAnim = ToriUtils.EnsureEmptyAnimExists();
                var stateMachine = ToriUtils.CreateLayer(_animator, _layerName).stateMachine;
                ToriUtils.CreateState(stateMachine, "Idle", ToriUtils.IdleStatePos, makeStatesWD, _emptyAnim, true);
            } finally {
                AssetDatabase.SaveAssets();
                AssetDatabase.StopAssetEditing();
                AssetDatabase.Refresh();
            }
        }
    }
}
#endif
