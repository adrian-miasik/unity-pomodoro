using UnityEditor;
using UnityEngine;

namespace LeTai.TrueShadow.Editor
{
[CustomPropertyDrawer(typeof(InsetToggleAttribute))]
public class InsetToggle : InlineToolbar
{
    static readonly Texture OUTER_SHADOW_TEXTURE = Utility.FindEditorResource<Texture>("Outer Shadow");
    static readonly Texture INNER_SHADOW_TEXTURE = Utility.FindEditorResource<Texture>("Inner Shadow");

    static InsetToggle()
    {
        textures = new[] {OUTER_SHADOW_TEXTURE, INNER_SHADOW_TEXTURE};
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return OUTER_SHADOW_TEXTURE.height + 4;
    }
}
}
