using AdrianMiasik.Components;
using Unity.VectorGraphics;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine;

namespace AdrianMiasik
{
    [CustomEditor(typeof(InformationToggle))]
    public class InformationToggleEditor : ToggleEditor 
    {
        public override void OnInspectorGUI()
        {
            // Define style
            GUIStyle style = new GUIStyle();
            style.fontStyle = FontStyle.Bold;
            style.normal.textColor = Color.white;
            
            // Draw title
            EditorGUILayout.LabelField("Toggle", style);
            
            base.OnInspectorGUI();
            
            // Fetch target script
            InformationToggle informationToggle = (InformationToggle) target;

            #region Vertical Group
            EditorGUILayout.BeginVertical();
            EditorGUILayout.Space();

            // Draw title
            EditorGUILayout.LabelField("Information Button", style);

            // Draw property fields
            informationToggle.icon = (SVGImage) EditorGUILayout.ObjectField("Icon", informationToggle.icon, typeof(SVGImage), true);
            informationToggle.falseSprite = (Sprite) EditorGUILayout.ObjectField("False Sprite", informationToggle.falseSprite, typeof(Sprite), true);
            informationToggle.falseColor = EditorGUILayout.ColorField("False Sprite Color", informationToggle.falseColor);
            informationToggle.falseZRotation = EditorGUILayout.FloatField("False Z Rotation", informationToggle.falseZRotation);
            informationToggle.trueSprite = (Sprite) EditorGUILayout.ObjectField("True Sprite", informationToggle.trueSprite, typeof(Sprite), true);
            informationToggle.trueZRotation = EditorGUILayout.FloatField("True Z Rotation", informationToggle.trueZRotation);
            informationToggle.trueColor = EditorGUILayout.ColorField("True Sprite Color", informationToggle.trueColor);
            
            // Draw UnityEvents
            SerializedProperty onFalseClick = serializedObject.FindProperty("OnSetToFalseClick");
            EditorGUILayout.PropertyField(onFalseClick);
            
            SerializedProperty onTrueClick = serializedObject.FindProperty("OnSetToTrueClick");
            EditorGUILayout.PropertyField(onTrueClick);
            
            serializedObject.ApplyModifiedProperties();
            
            EditorGUILayout.EndVertical();
            #endregion
        }
    }
}