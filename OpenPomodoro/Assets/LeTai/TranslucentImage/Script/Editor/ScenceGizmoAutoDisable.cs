using System;
using System.Linq;
using System.Reflection;

namespace LeTai.Asset.TranslucentImage.Editor
{
public static class ScenceGizmoAutoDisable
{
    static readonly string[] NO_GIZMOS_CLASSES = {
        "TranslucentImage",
        "TranslucentImageSource"
    };

    [UnityEditor.Callbacks.DidReloadScripts]
    private static void OnScriptsReloaded()
    {
        var Annotation = Type.GetType("UnityEditor.Annotation, UnityEditor");
        if (Annotation == null) return;

        var ClassId     = Annotation.GetField("classID");
        var ScriptClass = Annotation.GetField("scriptClass");
        var Flags       = Annotation.GetField("flags");
        var IconEnabled = Annotation.GetField("iconEnabled");

        Type AnnotationUtility = Type.GetType("UnityEditor.AnnotationUtility, UnityEditor");
        if (AnnotationUtility == null) return;

        var GetAnnotations = AnnotationUtility.GetMethod("GetAnnotations",
                                                         BindingFlags.NonPublic | BindingFlags.Public |
                                                         BindingFlags.Static);
        if (GetAnnotations == null) return;
        var SetIconEnabled = AnnotationUtility.GetMethod("SetIconEnabled",
                                                         BindingFlags.NonPublic | BindingFlags.Public |
                                                         BindingFlags.Static);
        if (SetIconEnabled == null) return;


        Array annotations = (Array) GetAnnotations.Invoke(null, null);
        foreach (var a in annotations)
        {
            int    classId     = (int) ClassId.GetValue(a);
            string scriptClass = (string) ScriptClass.GetValue(a);
            int    flags       = (int) Flags.GetValue(a);
            int    iconEnabled = (int) IconEnabled.GetValue(a);

            // built in types
            if (string.IsNullOrEmpty(scriptClass)) continue;

            // load a json or text file with class names

            const int HasIcon     = 1;
            bool      hasIconFlag = (flags & HasIcon) == HasIcon;

            if (hasIconFlag &&
                iconEnabled != 0 &&
                NO_GIZMOS_CLASSES.Contains(scriptClass))
            {
                SetIconEnabled.Invoke(null, new object[] {classId, scriptClass, 0});
            }
        }
    }
}
}
