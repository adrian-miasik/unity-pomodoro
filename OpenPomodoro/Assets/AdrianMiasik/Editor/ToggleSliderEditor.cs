using AdrianMiasik.Components.Core;
using UnityEditor;
using UnityEditor.UI;

namespace AdrianMiasik.Editor
{
    [CustomEditor(typeof(ToggleSlider))]
    [CanEditMultipleObjects]
    public class ToggleSliderEditor : ToggleEditor
    {
        // Properties
        private SerializedProperty background;
        private SerializedProperty dot;
        private SerializedProperty animation;
        private SerializedProperty leftToRight;
        private SerializedProperty rightToLeft;
        
        private SerializedProperty onSetToTrueClick;
        private SerializedProperty onSetToFalseClick;
        private SerializedProperty onClick;

        protected override void OnEnable()
        {
            base.OnEnable();

            background = serializedObject.FindProperty("m_background");
            dot = serializedObject.FindProperty("m_dot");
            animation = serializedObject.FindProperty("m_animation");
            leftToRight = serializedObject.FindProperty("m_leftToRight");
            rightToLeft = serializedObject.FindProperty("m_rightToLeft");
            onSetToTrueClick = serializedObject.FindProperty("m_onSetToTrueClick");
            onSetToFalseClick = serializedObject.FindProperty("m_onSetToFalseClick");
            onClick = serializedObject.FindProperty("m_onClick");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            serializedObject.Update();
            
            // Draw title
            EditorGUILayout.LabelField("Toggle Slider");
            
            // Properties
            EditorGUILayout.PropertyField(background);
            EditorGUILayout.PropertyField(dot);
            EditorGUILayout.PropertyField(animation);
            EditorGUILayout.PropertyField(leftToRight);
            EditorGUILayout.PropertyField(rightToLeft);

            // Unity Events
            EditorGUILayout.PropertyField(onSetToTrueClick);
            EditorGUILayout.PropertyField(onSetToFalseClick);
            EditorGUILayout.PropertyField(onClick);

            serializedObject.ApplyModifiedProperties();
        }
    }
}