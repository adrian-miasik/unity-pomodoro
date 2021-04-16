using UnityEditor;
using UnityEditor.UI;
using UnityEngine;

namespace AdrianMiasik
{
    [CustomEditor(typeof(ClickButton))]
    public class ClickButtonEditor : ImageEditor
    {
        public override void OnInspectorGUI()
        {
            // Define style
            GUIStyle style = new GUIStyle();
            style.fontStyle = FontStyle.Bold;
            style.normal.textColor = Color.white;
            
            // Draw title
            EditorGUILayout.LabelField("Image", style);
            
            base.OnInspectorGUI();
            
            // Fetch target script
            ClickButton clickButton = (ClickButton) target;

            #region Vertical Group
            EditorGUILayout.BeginVertical();
            EditorGUILayout.Space();

            // Draw title
            EditorGUILayout.LabelField("Click Button", style);

            // Draw property fields
            clickButton.target = (RectTransform) EditorGUILayout.ObjectField("Target", clickButton.target, typeof(RectTransform), true);
            clickButton.clickedDownScale = EditorGUILayout.FloatField("Clicked Down Scale", clickButton.clickedDownScale);
            clickButton.clickReleaseScale = EditorGUILayout.CurveField("Click Release Scale", clickButton.clickReleaseScale);

            // Draw OnClick UnityEvent
            SerializedProperty prop = serializedObject.FindProperty("OnClick");
            EditorGUILayout.PropertyField(prop);
            serializedObject.ApplyModifiedProperties();
            
            EditorGUILayout.EndVertical();
            #endregion
        }
    }
}
