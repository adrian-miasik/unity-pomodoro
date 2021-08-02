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
            clickButton.containerTarget = (RectTransform) EditorGUILayout.ObjectField("Container Target", clickButton.containerTarget, typeof(RectTransform), true);
            clickButton.visibilityTarget = (Transform) EditorGUILayout.ObjectField("Visibility Target", clickButton.visibilityTarget, typeof(RectTransform), true);
            clickButton.clickSound = (AudioSource) EditorGUILayout.ObjectField("Click Sound", clickButton.clickSound, typeof(AudioSource), true);
            string pitchVariationString = clickButton.isPitchVariationOn ? "Disable Pitch Variation" : "Enable Pitch Variation";
            clickButton.isPitchVariationOn = EditorGUILayout.Toggle(pitchVariationString, clickButton.isPitchVariationOn);
            if (clickButton.isPitchVariationOn)
            {
                clickButton.lowestPitch = EditorGUILayout.FloatField("Lowest Pitch", clickButton.lowestPitch);
                clickButton.highestPitch = EditorGUILayout.FloatField("Highest Pitch", clickButton.highestPitch);
                EditorGUILayout.MinMaxSlider("Pitch Variation", ref clickButton.lowestPitch, ref clickButton.highestPitch, 0, 2);
                EditorGUILayout.Space();
            }
            clickButton.clickHoldScale = EditorGUILayout.FloatField("Click Hold Scale", clickButton.clickHoldScale);
            clickButton.clickReleaseScale = EditorGUILayout.CurveField("Click Release Scale", clickButton.clickReleaseScale);
            clickButton.holdRamp = EditorGUILayout.CurveField("Hold Ramp", clickButton.holdRamp);

            // Draw UnityEvents
            SerializedProperty onDown = serializedObject.FindProperty("OnDown");
            EditorGUILayout.PropertyField(onDown);

            SerializedProperty onUp = serializedObject.FindProperty("OnUp");
            EditorGUILayout.PropertyField(onUp);
            
            SerializedProperty onClick = serializedObject.FindProperty("OnClick");
            EditorGUILayout.PropertyField(onClick);

            serializedObject.ApplyModifiedProperties();
            
            EditorGUILayout.EndVertical();
            #endregion
        }
    }
}
