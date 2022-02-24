using AdrianMiasik.Components.Core;
using UnityEditor;

namespace AdrianMiasik.Editor
{
    [CustomEditor(typeof(Dropdown), true)]
    [CanEditMultipleObjects]
    public class DropdownEditor: TMPro.EditorUtilities.DropdownEditor
    {
        private SerializedProperty audioSource;
        
        protected override void OnEnable()
        {
            base.OnEnable();
            audioSource = serializedObject.FindProperty("m_audio");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedObject.Update();
            EditorGUILayout.PropertyField(audioSource);
            serializedObject.ApplyModifiedProperties();
        }
    }
}