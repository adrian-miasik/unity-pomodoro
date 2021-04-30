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
            informationToggle.offSprite = (Sprite) EditorGUILayout.ObjectField("Off Sprite", informationToggle.offSprite, typeof(Sprite), true);
            informationToggle.offColor = (Color) EditorGUILayout.ColorField("Off Sprite Color", informationToggle.offColor);
            informationToggle.offZRotation = EditorGUILayout.FloatField("Off Z Rotation", informationToggle.offZRotation);
            informationToggle.onSprite = (Sprite) EditorGUILayout.ObjectField("On Sprite", informationToggle.onSprite, typeof(Sprite), true);
            informationToggle.onZRotation = EditorGUILayout.FloatField("On Z Rotation", informationToggle.onZRotation);
            informationToggle.onColor = (Color) EditorGUILayout.ColorField("On Sprite Color", informationToggle.onColor);
            
            serializedObject.ApplyModifiedProperties();
            
            EditorGUILayout.EndVertical();
            #endregion
        }
    }
}