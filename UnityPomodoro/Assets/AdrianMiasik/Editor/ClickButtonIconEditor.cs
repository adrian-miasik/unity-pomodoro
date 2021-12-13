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
            
            iconButton.icon = (SVGImage) EditorGUILayout.ObjectField("Icon", iconButton.icon, typeof(SVGImage), true);
        }
    }
}