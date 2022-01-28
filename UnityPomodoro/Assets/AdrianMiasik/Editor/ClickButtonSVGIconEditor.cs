using AdrianMiasik.Components.Base;
using AdrianMiasik.Components.Core;
using Unity.VectorGraphics;
using UnityEditor;

namespace AdrianMiasik.Editor
{
    [CustomEditor(typeof(ClickButtonSVGIcon))]
    public class ClickButtonSVGIconEditor: ClickButtonEditor
    {
        protected override void DrawInheritorFields(ClickButton _clickButton)
        {
            // Fetch target script
            ClickButtonSVGIcon svgIconButtonSvg = (ClickButtonSVGIcon) target;
            
            svgIconButtonSvg.m_icon = (SVGImage) EditorGUILayout.ObjectField("Icon", svgIconButtonSvg.m_icon, typeof(SVGImage), true);
        }
    }
}