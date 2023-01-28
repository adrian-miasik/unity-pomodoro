using AdrianMiasik.Components.Base;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine;

namespace AdrianMiasik.Editor
{
    [CustomEditor(typeof(ClickButton))]
    public class ClickButtonEditor : ImageEditor
    {
        // Properties
        private SerializedProperty containerTarget;
        private SerializedProperty enableClickSound;
        private SerializedProperty clickSound;
        private SerializedProperty isPitchVariationOn;
        private SerializedProperty lowestPitch;
        private SerializedProperty highestPitch;
        private SerializedProperty clickHoldScale;
        private SerializedProperty clickReleaseScale;
        private SerializedProperty holdRamp;
        private SerializedProperty isHoldable;
        
        // Unity Events
        private SerializedProperty onDown;
        private SerializedProperty onUp;
        private SerializedProperty onClick;

        protected override void OnEnable()
        {
            base.OnEnable();

            containerTarget = serializedObject.FindProperty("m_containerTarget");
            enableClickSound = serializedObject.FindProperty("m_enableClickSound");
            clickSound = serializedObject.FindProperty("m_clickSound");
            isPitchVariationOn = serializedObject.FindProperty("m_isPitchVariationOn");
            lowestPitch = serializedObject.FindProperty("m_lowestPitch");
            highestPitch = serializedObject.FindProperty("m_highestPitch");
            clickHoldScale = serializedObject.FindProperty("m_clickHoldScale");
            clickReleaseScale = serializedObject.FindProperty("m_clickReleaseScale");
            holdRamp = serializedObject.FindProperty("m_holdRamp");
            isHoldable = serializedObject.FindProperty("m_isHoldable");
            
            onDown = serializedObject.FindProperty("m_onDown");
            onUp = serializedObject.FindProperty("m_onUp");
            onClick = serializedObject.FindProperty("m_onClick");
        }

        public override void OnInspectorGUI()
        {
            // Define style
            GUIStyle style = new GUIStyle
            {
                fontStyle = FontStyle.Bold,
                normal =
                {
                    textColor = Color.white
                }
            };

            // Draw title
            EditorGUILayout.LabelField("Image", style);
            
            base.OnInspectorGUI();
            serializedObject.Update();
            
            // Open formatting
            EditorGUILayout.BeginVertical();
            EditorGUILayout.Space();

            // Draw title
            EditorGUILayout.LabelField("Click Button", style);

            // Fetch target script
            ClickButton clickButton = (ClickButton) target;
            
            // Properties
            EditorGUILayout.PropertyField(containerTarget);
            if (enableClickSound.boolValue)
            {
                EditorGUILayout.PropertyField(clickSound);
                EditorGUILayout.PropertyField(isPitchVariationOn);
                
                if (isPitchVariationOn.boolValue)
                {
                    EditorGUILayout.PropertyField(lowestPitch);
                    EditorGUILayout.PropertyField(highestPitch);
                    EditorGUILayout.MinMaxSlider("Pitch Variation", ref clickButton.m_lowestPitch, ref clickButton.m_highestPitch, 0, 2);
                    EditorGUILayout.Space();    
                }
            }
            EditorGUILayout.PropertyField(clickHoldScale);
            EditorGUILayout.PropertyField(clickReleaseScale);
            EditorGUILayout.PropertyField(holdRamp);
            EditorGUILayout.PropertyField(isHoldable);

            // Draw inherited variation class property fields
            DrawInheritorFields();

            // Unity Events
            EditorGUILayout.PropertyField(onDown);
            EditorGUILayout.PropertyField(onUp);
            EditorGUILayout.PropertyField(onClick);
            
            // Close formatting
            EditorGUILayout.EndVertical();

            serializedObject.ApplyModifiedProperties();
        }

        protected virtual void DrawInheritorFields()
        {
            // Nothing by default
        }
    }
}
