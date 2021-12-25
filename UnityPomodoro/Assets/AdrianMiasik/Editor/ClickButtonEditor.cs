using AdrianMiasik.Components.Core;
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
            
            // Open formatting
            EditorGUILayout.BeginVertical();
            EditorGUILayout.Space();

            // Draw title
            EditorGUILayout.LabelField("Click Button", style);

            // Draw fields
            DrawPropertyFields();
            
            // Close formatting
            EditorGUILayout.EndVertical();
        }

        private void DrawPropertyFields()
        {
            // Fetch target script
            ClickButton clickButton = (ClickButton) target;
            
            // Draw property fields
            clickButton.m_containerTarget = (RectTransform) EditorGUILayout.ObjectField("Container Target", clickButton.m_containerTarget, typeof(RectTransform), true);
            clickButton.m_enableClickSound = EditorGUILayout.Toggle("Enable Click Sound", clickButton.m_enableClickSound);
            clickButton.m_clickSound = (AudioSource) EditorGUILayout.ObjectField("Click Sound", clickButton.m_clickSound, typeof(AudioSource), true);
            string pitchVariationString = clickButton.m_isPitchVariationOn ? "Disable Pitch Variation" : "Enable Pitch Variation";
            clickButton.m_isPitchVariationOn = EditorGUILayout.Toggle(pitchVariationString, clickButton.m_isPitchVariationOn);
            if (clickButton.m_isPitchVariationOn)
            {
                clickButton.m_lowestPitch = EditorGUILayout.FloatField("Lowest Pitch", clickButton.m_lowestPitch);
                clickButton.m_highestPitch = EditorGUILayout.FloatField("Highest Pitch", clickButton.m_highestPitch);
                EditorGUILayout.MinMaxSlider("Pitch Variation", ref clickButton.m_lowestPitch, ref clickButton.m_highestPitch, 0, 2);
                EditorGUILayout.Space();
            }
            clickButton.m_clickHoldScale = EditorGUILayout.FloatField("Click Hold Scale", clickButton.m_clickHoldScale);
            clickButton.m_clickReleaseScale = EditorGUILayout.CurveField("Click Release Scale", clickButton.m_clickReleaseScale);
            clickButton.m_holdRamp = EditorGUILayout.CurveField("Hold Ramp", clickButton.m_holdRamp);

            // Draw inherited variation class property fields
            DrawInheritorFields(clickButton);

            // Draw UnityEvents
            SerializedProperty onDown = serializedObject.FindProperty("m_onDown");
            EditorGUILayout.PropertyField(onDown);

            SerializedProperty onUp = serializedObject.FindProperty("m_onUp");
            EditorGUILayout.PropertyField(onUp);
            
            SerializedProperty onClick = serializedObject.FindProperty("m_onClick");
            EditorGUILayout.PropertyField(onClick);

            serializedObject.ApplyModifiedProperties();
        }

        protected virtual void DrawInheritorFields(ClickButton button)
        {
            // Nothing by default
        }
    }
}
