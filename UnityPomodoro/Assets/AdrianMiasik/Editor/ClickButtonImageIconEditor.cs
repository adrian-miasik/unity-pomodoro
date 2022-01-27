using AdrianMiasik.Components.Base;
using AdrianMiasik.Components.Core;
using Unity.VectorGraphics;
using UnityEditor;
using UnityEngine.UI;

namespace AdrianMiasik.Editor
{
    [CustomEditor(typeof(ClickButtonImageIcon))]
    public class ClickButtonImageIconEditor: ClickButtonEditor
    {
        protected override void DrawInheritorFields(ClickButton _clickButton)
        {
            // Fetch target script
            ClickButtonImageIcon imageIconButton = (ClickButtonImageIcon) target;
            
            imageIconButton.m_icon = (Image) EditorGUILayout.ObjectField("Icon", imageIconButton.m_icon, typeof(Image), true);
        }
    }
}