using System;
using System.IO;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using static UnityEditor.EditorGUILayout;
using static UnityEditor.EditorGUIUtility;

namespace LeTai.TrueShadow.Editor
{
[CanEditMultipleObjects]
[CustomEditor(typeof(ProjectSettings))]
public class ProjectSettingsEditor : UnityEditor.Editor
{
    EditorProperty  useGlobalAngleByDefault;
    EditorProperty  globalAngle;
    EditorProperty  showQuickPresetsButtons;
    EditorProperty  quickPresets;
    ReorderableList reorderableList;

    void OnEnable()
    {
        // Domain reload cause target to be null
        if(target == null) return;

        useGlobalAngleByDefault = new EditorProperty(serializedObject, nameof(ProjectSettings.UseGlobalAngleByDefault));
        globalAngle             = new EditorProperty(serializedObject, nameof(ProjectSettings.GlobalAngle));
        showQuickPresetsButtons = new EditorProperty(serializedObject, nameof(ProjectSettings.ShowQuickPresetsButtons));
        quickPresets            = new EditorProperty(serializedObject, nameof(ProjectSettings.QuickPresets));

        reorderableList = new ReorderableList(serializedObject, quickPresets.serializedProperty, true, true, true, true) {
            drawHeaderCallback  = DrawPresetsHListHeader,
            drawElementCallback = DrawPresetListItems,
            elementHeight = singleLineHeight * 6
                          + standardVerticalSpacing * 7,
        };
    }

    void DrawPresetsHListHeader(Rect rect)
    {
        EditorGUI.PrefixLabel(rect, new GUIContent(quickPresets.serializedProperty.displayName));
    }

    void DrawPresetListItems(Rect rect, int index, bool isActive, bool isFocused)
    {
        SerializedProperty element = reorderableList.serializedProperty.GetArrayElementAtIndex(index);

        var childRect = new Rect(rect) { height = singleLineHeight };
        EditorGUI.LabelField(childRect, element.FindPropertyRelative(nameof(QuickPreset.name)).stringValue);
        childRect.y += singleLineHeight + standardVerticalSpacing;

        var oldLabelWidth = labelWidth;
        labelWidth = Mathf.Min(labelWidth, pixelsPerPoint * 60);
        foreach (var childProp in element)
        {
            EditorGUI.PropertyField(childRect, (SerializedProperty)childProp, true);
            childRect.y += singleLineHeight + standardVerticalSpacing;
        }

        labelWidth = oldLabelWidth;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        useGlobalAngleByDefault.Draw();
        globalAngle.Draw();
        Space();
        showQuickPresetsButtons.Draw();

        using (new GUILayout.HorizontalScope())
        {
            Space(pixelsPerPoint * 8, false);
            using (new GUILayout.VerticalScope(GUILayout.MaxWidth(pixelsPerPoint * 400)))
                reorderableList.DoLayoutList();
            Space(pixelsPerPoint * 8, false);
        }

        serializedObject.ApplyModifiedProperties();
    }

    [SettingsProvider]
    public static SettingsProvider CreatSettingsProvider()
    {
        if (!Resources.Load(ProjectSettings.RESOURCE_PATH))
            return null;

        return AssetSettingsProvider.CreateProviderFromResourcePath(
            "Project/True Shadow",
            ProjectSettings.RESOURCE_PATH,
            SettingsProvider.GetSearchKeywordsFromPath(ProjectSettings.RESOURCE_PATH)
        );
    }
}

class RunOnImport : AssetPostprocessor
{
    static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        if (Resources.Load(ProjectSettings.RESOURCE_PATH))
            return;

        foreach (var path in importedAssets)
        {
            var fileName = Path.GetFileNameWithoutExtension(path);

            if (string.Compare(fileName,
                               ProjectSettings.DEFAULT_RESOURCE_PATH,
                               StringComparison.InvariantCulture) == 0)
            {
                EnsureSettingAsset(path);
                break;
            }

            if (string.Compare(fileName,
                               ProjectSettings.RESOURCE_PATH,
                               StringComparison.InvariantCulture) == 0)
            {
                EditorApplication.update += NotifySettingsProviderChanged;
                break;
            }
        }
    }

    static void NotifySettingsProviderChanged()
    {
        SettingsService.NotifySettingsProviderChanged();

        EditorApplication.update -= NotifySettingsProviderChanged;
    }

    static void EnsureSettingAsset(string defaultPath)
    {
        // ReSharper disable once AssignNullToNotNullAttribute
        var userPath     = Path.Combine(Path.GetDirectoryName(defaultPath), ProjectSettings.RESOURCE_PATH + ".asset");
        var userSettings = AssetDatabase.LoadAssetAtPath<ProjectSettings>(userPath);
        if (userSettings)
            return;

        AssetDatabase.CopyAsset(defaultPath, userPath);
        AssetDatabase.ImportAsset(userPath,
                                  ImportAssetOptions.ForceUpdate
                                | ImportAssetOptions.ForceSynchronousImport);
    }
}
}
