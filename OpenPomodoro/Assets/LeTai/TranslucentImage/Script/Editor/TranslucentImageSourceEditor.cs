using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LeTai.Asset.TranslucentImage.Editor
{
[CustomEditor(typeof(TranslucentImageSource))]
[CanEditMultipleObjects]
public class TranslucentImageSourceEditor : UnityEditor.Editor
{
#region constants
    readonly GUIContent downsampleLabel = new GUIContent("Downsample",
                                                         "Reduce the size of the screen before processing. Increase will improve performance but create more artifact");

    readonly GUIContent regionLabel = new GUIContent("Blur Region",
                                                     "Choose which part of the screen to blur. Blur smaller region is faster");

    readonly GUIContent updateRateLabel = new GUIContent("Max Update Rate",
                                                         "How many time to blur per second. Reduce to increase performance and save battery for slow moving background");

    readonly GUIContent previewLabel = new GUIContent("Preview",
                                                      "Preview the effect over the entire screen");
#endregion


    UnityEditor.Editor configEditor;

    public ScalableBlurConfigEditor ConfigEditor
    {
        get
        {
            if (configEditor == null)
            {
                var config = ((TranslucentImageSource)target).BlurConfig;
                if (config != null)
                    configEditor = CreateEditor(config);
            }

            return (ScalableBlurConfigEditor)configEditor;
        }
    }

    public override void OnInspectorGUI()
    {
        Undo.RecordObject(target, "Change Translucent Image Source property");
        PrefabUtility.RecordPrefabInstancePropertyModifications(target);

        var tiSource = (TranslucentImageSource)target;

        using (var v = new EditorGUILayout.VerticalScope())
        {
            EditorGUILayout.Space();
            GUI.Box(v.rect, GUIContent.none);
            tiSource.BlurConfig = (BlurConfig)EditorGUILayout.ObjectField("Config file",
                                                                          tiSource.BlurConfig,
                                                                          typeof(BlurConfig),
                                                                          false);

            if (tiSource.BlurConfig == null)
            {
                EditorGUILayout.HelpBox("Missing Blur Config", MessageType.Warning);
                if (GUILayout.Button("New Blur Config File"))
                {
                    ScalableBlurConfig config = CreateInstance<ScalableBlurConfig>();

                    var path = AssetDatabase.GenerateUniqueAssetPath(
                        $"Assets/{SceneManager.GetActiveScene().name} {tiSource.gameObject.name} Blur Config.asset");
                    AssetDatabase.CreateAsset(config, path);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                    EditorGUIUtility.PingObject(config);
                    tiSource.BlurConfig = config;
                }
            }
            else
            {
                // EditorGUILayout.LabelField("Blur settings", EditorStyles.centeredGreyMiniLabel);
                ConfigEditor.Draw((ScalableBlurConfig)tiSource.BlurConfig);
            }
        }

        EditorGUILayout.Space();

        // Common properties
        tiSource.Downsample    = EditorGUILayout.IntSlider(downsampleLabel, tiSource.Downsample, 0, 3);
        tiSource.BlurRegion    = EditorGUILayout.RectField(regionLabel, tiSource.BlurRegion);
        tiSource.maxUpdateRate = EditorGUILayout.FloatField(updateRateLabel, tiSource.maxUpdateRate);
        tiSource.preview       = EditorGUILayout.Toggle(previewLabel, tiSource.preview);

        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
        }
    }
}
}
