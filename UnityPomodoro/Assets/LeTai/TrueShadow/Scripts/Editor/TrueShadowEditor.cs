using System.Linq;
using System.Reflection;
using LeTai.TrueShadow.PluginInterfaces;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.EditorGUILayout;

namespace LeTai.TrueShadow.Editor
{
[CanEditMultipleObjects]
[CustomEditor(typeof(TrueShadow))]
public class TrueShadowEditor : UnityEditor.Editor
{
    EditorProperty insetProp;
    EditorProperty sizeProp;
    EditorProperty spreadProp;
    EditorProperty useGlobalAngleProp;
    EditorProperty angleProp;
    EditorProperty distanceProp;
    EditorProperty colorProp;
    EditorProperty blendModeProp;
    EditorProperty multiplyCasterAlphaProp;
    EditorProperty ignoreCasterColorProp;
    EditorProperty colorBleedModeProp;
    EditorProperty disableFitCompensationProp;

#if LETAI_TRUESHADOW_DEBUG
    SerializedProperty alwayRenderProp;
#endif

    GUIContent procrastinateLabel;
    GUIContent editGlobalAngleLabel;

    static bool showExperimental;
    static bool showAdvanced;

    static Texture    warningIcon;
    static GUIStyle   hashWarningStyle;
    static GUIContent hashWarningLabel;

    void OnEnable()
    {
        insetProp                  = new EditorProperty(serializedObject, nameof(TrueShadow.Inset));
        sizeProp                   = new EditorProperty(serializedObject, nameof(TrueShadow.Size));
        spreadProp                 = new EditorProperty(serializedObject, nameof(TrueShadow.Spread));
        useGlobalAngleProp         = new EditorProperty(serializedObject, nameof(TrueShadow.UseGlobalAngle));
        angleProp                  = new EditorProperty(serializedObject, nameof(TrueShadow.OffsetAngle));
        distanceProp               = new EditorProperty(serializedObject, nameof(TrueShadow.OffsetDistance));
        colorProp                  = new EditorProperty(serializedObject, nameof(TrueShadow.Color));
        blendModeProp              = new EditorProperty(serializedObject, nameof(TrueShadow.BlendMode));
        multiplyCasterAlphaProp    = new EditorProperty(serializedObject, nameof(TrueShadow.UseCasterAlpha));
        ignoreCasterColorProp      = new EditorProperty(serializedObject, nameof(TrueShadow.IgnoreCasterColor));
        colorBleedModeProp         = new EditorProperty(serializedObject, nameof(TrueShadow.ColorBleedMode));
        disableFitCompensationProp = new EditorProperty(serializedObject, nameof(TrueShadow.DisableFitCompensation));

#if LETAI_TRUESHADOW_DEBUG
        alwayRenderProp = serializedObject.FindProperty(nameof(TrueShadow.alwaysRender));
#endif

        if (EditorPrefs.GetBool("LeTai_TrueShadow_" + nameof(showExperimental)))
        {
            showExperimental = EditorPrefs.GetBool("LeTai_TrueShadow_" + nameof(showExperimental), false);
            showAdvanced     = EditorPrefs.GetBool("LeTai_TrueShadow_" + nameof(showAdvanced),     false);
        }

        procrastinateLabel   = new GUIContent("Procrastinate", "A bug that is too fun to fix");
        editGlobalAngleLabel = new GUIContent("Edit...");

        if (!warningIcon)
        {
            warningIcon = typeof(EditorGUIUtility)
                         .GetProperty("warningIcon", BindingFlags.Static | BindingFlags.NonPublic)
                        ?.GetValue(null) as Texture;
        }

        hashWarningLabel = new GUIContent(warningIcon);
        hashWarningStyle = new GUIStyle(EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector)
                                                        .FindStyle("WordWrappedLabel")) {
            richText = true
        };
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        var ts = (TrueShadow)target;

        DrawPresetButtons(ts);

        Space();

        insetProp.Draw();
        sizeProp.Draw();
        spreadProp.Draw();
        useGlobalAngleProp.Draw(GUILayout.ExpandWidth(!ts.UseGlobalAngle));
        if (ts.UseGlobalAngle)
        {
            var settingRect = GUILayoutUtility.GetLastRect();
            settingRect.xMin  += EditorGUIUtility.labelWidth + EditorGUIUtility.singleLineHeight;
            settingRect.width =  GUI.skin.button.CalcSize(editGlobalAngleLabel).x;
            if (GUI.Button(settingRect, editGlobalAngleLabel))
            {
                SettingsService.OpenProjectSettings("Project/True Shadow");
            }
        }
        else
        {
            angleProp.Draw();
        }

        distanceProp.Draw();
        colorProp.Draw();
        if (ts.UsingRendererMaterialProvider)
        {
            using (new EditorGUI.DisabledScope(true))
                LabelField(blendModeProp.serializedProperty.displayName, "Custom Material");
        }
        else
        {
            blendModeProp.Draw();
        }

        DrawAdvancedSettings();

        DrawHashWarning();

        serializedObject.ApplyModifiedProperties();
    }

    void DrawPresetButtons(TrueShadow ts)
    {
        if (!ProjectSettings.Instance.ShowQuickPresetsButtons) return;

        using (new HorizontalScope())
        {
            var presets  = ProjectSettings.Instance.QuickPresets;
            var selected = GUILayout.Toolbar(-1, presets.Select(p => p.name).ToArray());
            if (selected != -1)
            {
                Undo.RecordObject(ts, "Apply Quick Preset on " + ts.name);
                presets[selected].Apply(ts);
                EditorApplication.QueuePlayerLoopUpdate();
            }

            if (GUILayout.Button("...", GUILayout.ExpandWidth(false)))
            {
                SettingsService.OpenProjectSettings("Project/True Shadow");
            }
        }
    }

    void DrawAdvancedSettings()
    {
        using (var change = new EditorGUI.ChangeCheckScope())
        {
            showAdvanced = Foldout(showAdvanced, "Advanced Settings", true);
            using (new EditorGUI.IndentLevelScope())
                if (showAdvanced)
                {
                    multiplyCasterAlphaProp.Draw();
                    ignoreCasterColorProp.Draw();
                    colorBleedModeProp.Draw();
                    disableFitCompensationProp.Draw();

                    if (KnobPropertyDrawer.procrastinationMode)
                    {
                        var rot = GUI.matrix;
                        GUI.matrix                             =  Matrix4x4.identity;
                        KnobPropertyDrawer.procrastinationMode ^= Toggle("Be Productive", false);
                        GUI.matrix                             =  rot;
                    }
                    else
                    {
                        KnobPropertyDrawer.procrastinationMode |= Toggle(procrastinateLabel, false);
                    }

#if LETAI_TRUESHADOW_DEBUG
                    PropertyField(alwayRenderProp);
#endif
                }

            if (change.changed)
            {
                EditorPrefs.SetBool("LeTai_TrueShadow_" + nameof(showExperimental), showExperimental);
                EditorPrefs.SetBool("LeTai_TrueShadow_" + nameof(showAdvanced),     showAdvanced);
            }
        }
    }

    static readonly string[] KNOWN_TYPES = {
        "UnityEngine.UI.Image",
        "UnityEngine.UI.RawImage",
        "UnityEngine.UI.Text",
        "TMPro.TextMeshProUGUI",
        "Unity.VectorGraphics.SVGImage",
    };


    void DrawHashWarning()
    {
        var ts = (TrueShadow)target;

        if (ts.GetComponent<ITrueShadowCustomHashProvider>() != null)
            return;

        var casterType = ts.GetComponent<Graphic>().GetType();
        if (KNOWN_TYPES.Contains(casterType.FullName))
            return;

        hashWarningLabel.text = "Shadow may not update with changes";

        using (var _ = new VerticalScope(EditorStyles.helpBox))
        {
            GUILayout.Label(hashWarningLabel);
            GUILayout.Label($"True Shadow can't tell 2 <i>{casterType.Name}</i> apart." +
                            $" The shadow may not update when the <i>{casterType.Name}</i> changes.\n" +
                            $"To fix this, set the shadow CustomHash, or disable shadow caching for this element.",
                            hashWarningStyle);

            if (GUILayout.Button("More info on CustomHash", EditorStyles.linkLabel))
            {
                Application.OpenURL("https://leloctai.com/trueshadow/docs/articles/integration.html#make-sure-shadow-update");
            }

            if (GUILayout.Button("Disable Shadow Cache for this element", EditorStyles.linkLabel))
            {
                Undo.AddComponent<DisableShadowCache>(ts.gameObject);
            }
        }
    }
}
}
