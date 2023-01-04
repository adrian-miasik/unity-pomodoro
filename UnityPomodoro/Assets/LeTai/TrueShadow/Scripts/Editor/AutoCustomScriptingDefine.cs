using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace LeTai.TrueShadow.Editor
{
[InitializeOnLoad]
public static class AutoCustomScriptingDefine
{
    internal static readonly HashSet<string> SYMBOLS = new HashSet<string> {"LETAI_TRUESHADOW"};

    static AutoCustomScriptingDefine()
    {
        Apply();
    }

    public static void Apply()
    {
        AddMissingSymbols(EditorUserBuildSettings.activeBuildTarget);
    }

    static void AddMissingSymbols(BuildTarget buildTarget)
    {
        var currentGroup = BuildPipeline.GetBuildTargetGroup(buildTarget);
        var defines      = PlayerSettings.GetScriptingDefineSymbolsForGroup(currentGroup).Split(';').ToList();
        var missing      = SYMBOLS.Except(defines).ToList();
        defines.AddRange(missing);

        if (missing.Count > 0)
            PlayerSettings.SetScriptingDefineSymbolsForGroup(currentGroup, string.Join(";", defines));
    }
}
}
