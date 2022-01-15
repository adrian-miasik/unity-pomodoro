using AdrianMiasik.Components.Core;
using Unity.VectorGraphics;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine;

namespace AdrianMiasik.Editor
{
    [CustomEditor(typeof(ToggleSprite))]
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
            ToggleSprite toggleSprite = (ToggleSprite) target;

            #region Vertical Group
            EditorGUILayout.BeginVertical();
            EditorGUILayout.Space();

            // Draw title
            EditorGUILayout.LabelField("Information Button", _style);

            // Draw property fields
            toggleSprite.m_icon = (SVGImage) EditorGUILayout.ObjectField("Icon", toggleSprite.m_icon, typeof(SVGImage), true);
            toggleSprite.m_falseSprite = (Sprite) EditorGUILayout.ObjectField("False Sprite", toggleSprite.m_falseSprite, typeof(Sprite), true);
            toggleSprite.m_falseZRotation = EditorGUILayout.FloatField("False Z Rotation", toggleSprite.m_falseZRotation);
            toggleSprite.m_trueSprite = (Sprite) EditorGUILayout.ObjectField("True Sprite", toggleSprite.m_trueSprite, typeof(Sprite), true);
            toggleSprite.m_trueZRotation = EditorGUILayout.FloatField("True Z Rotation", toggleSprite.m_trueZRotation);
            
            // Draw UnityEvents
            SerializedProperty _onFalseClick = serializedObject.FindProperty("m_onSetToFalseClick");
            EditorGUILayout.PropertyField(_onFalseClick);
            
            SerializedProperty _onTrueClick = serializedObject.FindProperty("m_onSetToTrueClick");
            EditorGUILayout.PropertyField(_onTrueClick);
            
            serializedObject.ApplyModifiedProperties();
            
            EditorGUILayout.EndVertical();
            #endregion
        }
    }
}