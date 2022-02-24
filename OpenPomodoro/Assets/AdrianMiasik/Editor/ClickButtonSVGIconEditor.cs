using AdrianMiasik.Components.Core;
using UnityEditor;

namespace AdrianMiasik.Editor
{
    [CustomEditor(typeof(ClickButtonSVGIcon))]
    public class ClickButtonSVGIconEditor: ClickButtonEditor
    {
        private SerializedProperty clickButtonSVGIcon;
        
        protected override void OnEnable()
        {
            base.OnEnable();

            clickButtonSVGIcon = serializedObject.FindProperty("m_icon");
        }

        protected override void DrawInheritorFields()
        {
            EditorGUILayout.PropertyField(clickButtonSVGIcon);
        }
    }
}