using AdrianMiasik.Components.Core;
using Unity.VectorGraphics;
using UnityEditor;

namespace AdrianMiasik
{
    [CustomEditor(typeof(ClickButtonIcon))]
    public class ClickButtonIconEditor: ClickButtonEditor
    {
        protected override void DrawInheritorFields(ClickButton _clickButton)
        {
            // Fetch target script
            ClickButtonIcon iconButton = (ClickButtonIcon) target;
            
            iconButton.m_icon = (SVGImage) EditorGUILayout.ObjectField("Icon", iconButton.m_icon, typeof(SVGImage), true);
        }
    }
}