using AdrianMiasik.Components.Core;
using AdrianMiasik.Components.Wrappers;
using TMPro;
using Unity.VectorGraphics;
using UnityEditor;

namespace AdrianMiasik
{
    [CustomEditor(typeof(ClickButtonText))]
    public class ClickButtonTextEditor: ClickButtonEditor
    {
        protected override void DrawInheritorFields(ClickButton _clickButton)
        {
            // Fetch target script
            ClickButtonText textButton = (ClickButtonText) target;
            
            textButton.m_text = (TMP_Text) EditorGUILayout.ObjectField("Text", textButton.m_text, typeof(TMP_Text), true);
        }
    }
}