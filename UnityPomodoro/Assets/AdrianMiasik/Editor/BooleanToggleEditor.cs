using AdrianMiasik.Components;
using Unity.VectorGraphics;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine;

namespace AdrianMiasik
{
    [CustomEditor(typeof(BooleanToggle))]
    public class BooleanToggleEditor : ToggleEditor 
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
            BooleanToggle booleanToggle = (BooleanToggle) target;

            #region Vertical Group
            EditorGUILayout.BeginVertical();
            EditorGUILayout.Space();

            // Draw title
            EditorGUILayout.LabelField("Information Button", style);

            // Draw property fields
            booleanToggle.icon = (SVGImage) EditorGUILayout.ObjectField("Icon", booleanToggle.icon, typeof(SVGImage), true);
            booleanToggle.falseSprite = (Sprite) EditorGUILayout.ObjectField("False Sprite", booleanToggle.falseSprite, typeof(Sprite), true);
            booleanToggle.falseColor = EditorGUILayout.ColorField("False Sprite Color", booleanToggle.falseColor);
            booleanToggle.falseZRotation = EditorGUILayout.FloatField("False Z Rotation", booleanToggle.falseZRotation);
            booleanToggle.trueSprite = (Sprite) EditorGUILayout.ObjectField("True Sprite", booleanToggle.trueSprite, typeof(Sprite), true);
            booleanToggle.trueZRotation = EditorGUILayout.FloatField("True Z Rotation", booleanToggle.trueZRotation);
            booleanToggle.trueColor = EditorGUILayout.ColorField("True Sprite Color", booleanToggle.trueColor);
            
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