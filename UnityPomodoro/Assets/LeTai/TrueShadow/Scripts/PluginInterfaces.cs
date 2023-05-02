using System;
using UnityEngine;
using UnityEngine.UI;

namespace LeTai.TrueShadow.PluginInterfaces
{
public interface ITrueShadowCasterMaterialProvider
{
    event Action materialReplaced;
    event Action materialModified;
    Material     GetTrueShadowCasterMaterial();
}

public interface ITrueShadowCasterMeshModifier
{
    void ModifyTrueShadowCasterMesh(Mesh mesh);
}

public interface ITrueShadowCasterMaterialPropertiesModifier
{
    void ModifyTrueShadowCasterMaterialProperties(MaterialPropertyBlock propertyBlock);
}

public interface ITrueShadowCasterClearColorProvider
{
    Color GetTrueShadowCasterClearColor();
}

public interface ITrueShadowRendererMaterialProvider
{
    event Action materialReplaced;
    event Action materialModified;
    Material     GetTrueShadowRendererMaterial();
}

public interface ITrueShadowRendererMaterialModifier
{
    void ModifyTrueShadowRendererMaterial(Material baseMaterial);
}

public interface ITrueShadowRendererMeshModifier
{
    void ModifyTrueShadowRendererMesh(VertexHelper vertexHelper);
}

public interface ITrueShadowCustomHashProvider { }
}
