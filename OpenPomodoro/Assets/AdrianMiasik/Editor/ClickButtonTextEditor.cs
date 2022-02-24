using AdrianMiasik.Components.Core;
using UnityEditor;

namespace AdrianMiasik.Editor
{
    [CustomEditor(typeof(ClickButtonText))]
    public class ClickButtonTextEditor: ClickButtonEditor
    {
        private SerializedProperty text;

        protected override void OnEnable()
        {
            base.OnEnable();

            text = serializedObject.FindProperty("m_text");
        }

        protected override void DrawInheritorFields()
        {
            EditorGUILayout.PropertyField(text);
        }
    }
}