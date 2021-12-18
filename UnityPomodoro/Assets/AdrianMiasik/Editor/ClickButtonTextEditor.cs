using AdrianMiasik.Components.Core;
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
            
            textButton.text = (TMP_Text) EditorGUILayout.ObjectField("Text", textButton.text, typeof(TMP_Text), true);
        }
    }
}