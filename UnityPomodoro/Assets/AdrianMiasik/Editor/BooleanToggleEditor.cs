using AdrianMiasik.Components.Core;
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
            // ReSharper disable once UseObjectOrCollectionInitializer
            GUIStyle _style = new GUIStyle();
            _style.fontStyle = FontStyle.Bold;
            _style.normal.textColor = Color.white;
            
            // Draw title
            EditorGUILayout.LabelField("Toggle", _style);
            
            base.OnInspectorGUI();
            
            // Fetch target script
            BooleanToggle _booleanToggle = (BooleanToggle) target;

            #region Vertical Group
            EditorGUILayout.BeginVertical();
            EditorGUILayout.Space();

            // Draw title
            EditorGUILayout.LabelField("Information Button", _style);

            // Draw property fields
            _booleanToggle.icon = (SVGImage) EditorGUILayout.ObjectField("Icon", _booleanToggle.icon, typeof(SVGImage), true);
            _booleanToggle.falseSprite = (Sprite) EditorGUILayout.ObjectField("False Sprite", _booleanToggle.falseSprite, typeof(Sprite), true);
            _booleanToggle.falseZRotation = EditorGUILayout.FloatField("False Z Rotation", _booleanToggle.falseZRotation);
            _booleanToggle.trueSprite = (Sprite) EditorGUILayout.ObjectField("True Sprite", _booleanToggle.trueSprite, typeof(Sprite), true);
            _booleanToggle.trueZRotation = EditorGUILayout.FloatField("True Z Rotation", _booleanToggle.trueZRotation);
            
            // Draw UnityEvents
            SerializedProperty _onFalseClick = serializedObject.FindProperty("onSetToFalseClick");
            EditorGUILayout.PropertyField(_onFalseClick);
            
            SerializedProperty _onTrueClick = serializedObject.FindProperty("onSetToTrueClick");
            EditorGUILayout.PropertyField(_onTrueClick);
            
            serializedObject.ApplyModifiedProperties();
            
            EditorGUILayout.EndVertical();
            #endregion
        }
    }
}