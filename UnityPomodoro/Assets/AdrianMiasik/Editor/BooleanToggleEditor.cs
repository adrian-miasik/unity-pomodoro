using AdrianMiasik.Components.Core;
using Unity.VectorGraphics;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine;

namespace AdrianMiasik.Editor
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
            _booleanToggle.m_icon = (SVGImage) EditorGUILayout.ObjectField("Icon", _booleanToggle.m_icon, typeof(SVGImage), true);
            _booleanToggle.m_falseSprite = (Sprite) EditorGUILayout.ObjectField("False Sprite", _booleanToggle.m_falseSprite, typeof(Sprite), true);
            _booleanToggle.m_falseZRotation = EditorGUILayout.FloatField("False Z Rotation", _booleanToggle.m_falseZRotation);
            _booleanToggle.m_trueSprite = (Sprite) EditorGUILayout.ObjectField("True Sprite", _booleanToggle.m_trueSprite, typeof(Sprite), true);
            _booleanToggle.m_trueZRotation = EditorGUILayout.FloatField("True Z Rotation", _booleanToggle.m_trueZRotation);
            
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