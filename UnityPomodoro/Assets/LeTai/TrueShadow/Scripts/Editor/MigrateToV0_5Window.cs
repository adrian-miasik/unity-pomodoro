using System.Linq;
using UnityEditor;
using UnityEngine;

namespace LeTai.TrueShadow.Editor
{
public class MigrateToVV1Window : EditorWindow
{
    private const string SHOW_ON_START_EDITOR_PREFS_KEY = "LeTai.TrueShadow.MigrateToVV1WindowShown";

    [MenuItem("Tools/TrueShadow/Migrate To v1")]
    public static MigrateToVV1Window ShowWindow()
    {
        var window = GetWindow<MigrateToVV1Window>(true, "True Shadow");
        window.position = new Rect(600, 400, 600, 400);
        return window;
    }

    [InitializeOnLoadMethod]
    private static void InitializeOnLoadMethod()
    {
        RegisterWindowCheck();
    }

    private static void RegisterWindowCheck()
    {
        if (!EditorApplication.isPlayingOrWillChangePlaymode)
        {
            EditorApplication.update += CheckShowWindow;
        }
    }

    private static void CheckShowWindow()
    {
        EditorApplication.update -= CheckShowWindow;
        if (EditorPrefs.GetBool(SHOW_ON_START_EDITOR_PREFS_KEY, true))
        {
            ShowWindow();
        }
    }

    void OnDestroy()
    {
        EditorPrefs.SetBool(SHOW_ON_START_EDITOR_PREFS_KEY, false);
    }

    bool haveBackup;

    private void OnGUI()
    {
        GUILayout.Label("Migrate to v1", EditorStyles.largeLabel);
        EditorGUILayout.Separator();
        EditorGUILayout.HelpBox(
            "In v1, Blend Mode was changed to produce better looking shadows, as well as better compatibility with 3rd parties asset. As a side effect, most shadows should now use Color Bleed Mode: <Black>. This tool attempt to do this automatically.\n\n" +
            "All True Shadows in currently loaded scenes will be migrated. You may want to load all scenes you want to fix before migrating. All True Shadows in prefabs will also be migrated.\n\n" +
            "All True Shadows in prefabs will also be migrated.\n\n" +
            "You may access this dialog later from the Tools menu.",
            MessageType.Info
        );
        EditorGUILayout.Separator();
        EditorGUILayout.HelpBox(
            "!!! MAKE SURE TO BACK UP YOUR PROJECT BEFORE USE !!!\n\n" +
            "This tool will modify your project files. Please backup your project before use. If you are unsure how to do this, do NOT use this tool! Manually change any problematic shadows Color Bleed mode to Black instead!",
            MessageType.Warning);
        EditorGUILayout.Separator();
        haveBackup = EditorGUILayout.ToggleLeft("I have backed up the project and can undo any changes done by the tool", haveBackup);
        if (haveBackup)
        {
            if (GUILayout.Button("Migrate to v1"))
                MigrateToV1();
        }
    }

    public static void MigrateToV1()
    {
        var allPrefabs = AssetDatabase.FindAssets("t:Prefab");
        foreach (var guid in allPrefabs)
        {
            var path       = AssetDatabase.GUIDToAssetPath(guid);
            var prefabRoot = PrefabUtility.LoadPrefabContents(path);
            var changed    = false;

            foreach (var shadow in prefabRoot.GetComponentsInChildren<TrueShadow>())
            {
                shadow.ColorBleedMode = ColorBleedMode.Black;
                changed               = true;
            }

            if (changed)
                PrefabUtility.SaveAsPrefabAsset(prefabRoot, path);

            PrefabUtility.UnloadPrefabContents(prefabRoot);
        }

        var inScene = Resources.FindObjectsOfTypeAll<TrueShadow>()
                               .ToArray();
        Undo.RecordObjects(inScene, "Migrate to 0.5");
        foreach (var shadow in inScene)
        {
            shadow.ColorBleedMode = ColorBleedMode.Black;
        }
    }
}
}
