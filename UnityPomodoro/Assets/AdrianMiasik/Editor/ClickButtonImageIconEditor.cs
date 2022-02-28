using AdrianMiasik.Components.Core;
using UnityEditor;

namespace AdrianMiasik.Editor
{
    [CustomEditor(typeof(ClickButtonImageIcon))]
    public class ClickButtonImageIconEditor: ClickButtonEditor
    {
        private SerializedProperty clickButtonImageIcon;
        
        protected override void OnEnable()
        {
            base.OnEnable();

            clickButtonImageIcon = serializedObject.FindProperty("m_icon");
        }

        protected override void DrawInheritorFields()
        {
            EditorGUILayout.PropertyField(clickButtonImageIcon);
        }
    }
}