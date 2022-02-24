using AdrianMiasik.Components.Core;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine;

namespace AdrianMiasik.Editor
{
    [CustomEditor(typeof(ToggleSprite))]
    [CanEditMultipleObjects]
    public class BooleanToggleEditor : ToggleEditor
    {
        // Properties
        private SerializedProperty icon;
        private SerializedProperty falseSprite;
        private SerializedProperty falseZRotation;
        private SerializedProperty trueSprite;
        private SerializedProperty trueZRotation;
        
        // Unity Events
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
            GUIStyle style = new GUIStyle
            {
                fontStyle = FontStyle.Bold,
                normal =
                {
                    textColor = Color.white
                }
            };

            // Draw title
            EditorGUILayout.LabelField("Toggle", style);
            
            base.OnInspectorGUI();
            serializedObject.Update();
            
            EditorGUILayout.BeginVertical();
            EditorGUILayout.Space();

            // Draw title
            EditorGUILayout.LabelField("Information Button", style);
            
            // Properties
            EditorGUILayout.PropertyField(icon);
            EditorGUILayout.PropertyField(falseSprite);
            EditorGUILayout.PropertyField(falseZRotation);
            EditorGUILayout.PropertyField(trueSprite);
            EditorGUILayout.PropertyField(trueZRotation);
            
            // Unity Events
            EditorGUILayout.PropertyField(onTrueClick);
            EditorGUILayout.PropertyField(onFalseClick);
            
            EditorGUILayout.EndVertical();
            
            serializedObject.ApplyModifiedProperties();
        }
    }
}