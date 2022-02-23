using AdrianMiasik.Components.Core;
using Unity.VectorGraphics;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine;

namespace AdrianMiasik.Editor
{
    [CustomEditor(typeof(ToggleSprite))]
    [CanEditMultipleObjects]
    public class BooleanToggleEditor : ToggleEditor
    {
        private SerializedProperty icon;
        private SerializedProperty falseSprite;
        private SerializedProperty falseZRotation;
        private SerializedProperty trueSprite;
        private SerializedProperty trueZRotation;
        private SerializedProperty onTrueClick;
        private SerializedProperty onFalseClick;

        protected override void OnEnable()
        {
            base.OnEnable();
            
            icon = serializedObject.FindProperty("m_icon");
            falseSprite = serializedObject.FindProperty("m_falseSprite");
            falseZRotation = serializedObject.FindProperty("m_falseZRotation");
            trueSprite = serializedObject.FindProperty("m_trueSprite");
            trueZRotation = serializedObject.FindProperty("m_trueZRotation");
            
            onTrueClick = serializedObject.FindProperty("m_onSetToTrueClick");
            onFalseClick = serializedObject.FindProperty("m_onSetToFalseClick");
        }

        public override void OnInspectorGUI()
        {
            // Define style
            // ReSharper disable once UseObjectOrCollectionInitializer
            GUIStyle _style = new GUIStyle();
            _style.fontStyle = FontStyle.Bold;
            _style.normal.textColor = Color.white;
            
            // Draw title
            EditorGUILayout.LabelField("Toggle", _style);
            
            base.OnInspectorGUI();
            serializedObject.Update();
            
            #region Vertical Group
            EditorGUILayout.BeginVertical();
            EditorGUILayout.Space();

            // Draw title
            EditorGUILayout.LabelField("Information Button", _style);

            // Draw property fields
            EditorGUILayout.PropertyField(icon);
            EditorGUILayout.PropertyField(falseSprite);
            EditorGUILayout.PropertyField(falseZRotation);
            EditorGUILayout.PropertyField(trueSprite);
            EditorGUILayout.PropertyField(trueZRotation);
            EditorGUILayout.PropertyField(onTrueClick);
            EditorGUILayout.PropertyField(onFalseClick);

            serializedObject.ApplyModifiedProperties();

            EditorGUILayout.EndVertical();
            #endregion
        }
    }
}