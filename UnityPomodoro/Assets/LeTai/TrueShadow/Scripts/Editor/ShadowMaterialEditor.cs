using UnityEditor;
using UnityEngine;

namespace LeTai.TrueShadow.Editor
{
[CustomEditor(typeof(ShadowMaterial))]
public class ShadowMaterialEditor : UnityEditor.Editor
{
    ShadowMaterial shadowMaterial;
    MaterialEditor materialEditor;

    void OnEnable()
    {
        shadowMaterial = (ShadowMaterial) target;

        if (shadowMaterial.material != null)
        {
            materialEditor = (MaterialEditor) CreateEditor(shadowMaterial.material);
        }
    }

    public override void OnInspectorGUI()
    {
        EditorGUI.BeginChangeCheck();

        EditorGUILayout.PropertyField(serializedObject.FindProperty("material"));

        if (EditorGUI.EndChangeCheck())
        {
            serializedObject.ApplyModifiedProperties();

            if (materialEditor)
            {
                DestroyImmediate(materialEditor);
            }

            if (shadowMaterial.material)
            {
                materialEditor = (MaterialEditor) CreateEditor(shadowMaterial.material);
            }
        }


        if (materialEditor)
        {
            materialEditor.DrawHeader();

            bool isDefaultMaterial = !AssetDatabase.GetAssetPath(shadowMaterial.material).StartsWith("Assets");
            using (new EditorGUI.DisabledGroupScope(isDefaultMaterial))
            {
                EditorGUI.BeginChangeCheck();
                materialEditor.OnInspectorGUI();
                if (EditorGUI.EndChangeCheck())
                {
                    shadowMaterial.OnMaterialModified();
                }
            }
        }
    }

    void OnDisable()
    {
        if (materialEditor)
        {
            DestroyImmediate(materialEditor);
        }
    }
}
}
