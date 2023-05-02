using UnityEditor;
using UnityEngine;

namespace LeTai.TrueShadow.Editor
{
public static class Utility
{
    internal static T FindEditorResource<T>(string assetName) where T : Object
    {
        var guids = AssetDatabase.FindAssets("l:TrueShadowEditorResources " + assetName);
        if (guids.Length == 0)
        {
            Debug.LogError(
                $"Asset \"{assetName}\" not found. Make sure it have the label \"TrueShadowEditorResources\"");
            return null;
        }

        var path = AssetDatabase.GUIDToAssetPath(guids[0]);
        return AssetDatabase.LoadAssetAtPath<T>(path);
    }
}
}
