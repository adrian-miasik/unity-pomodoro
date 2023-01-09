#if LETAI_TRUESHADOW_DEBUG
using System;
using UnityEngine;
#if UNITY_EDITOR
using System.IO;
using UnityEditor;

#endif

namespace LeTai.TrueShadow
{
[Serializable]
public class DebugSettings
{
    const string FILE_PATH = "ProjectSettings/TrueShadowDebugSettings.json";

    static DebugSettings instance;

    public static DebugSettings Instance
    {
        get
        {
            if (instance == null)
            {
#if UNITY_EDITOR
                try
                {
                    if (File.Exists(FILE_PATH))
                        instance = JsonUtility.FromJson<DebugSettings>(File.ReadAllText(FILE_PATH));
                    else
                        Create();
                }
                catch (Exception)
                {
                    Create();
                }
#else
                Create();
#endif
            }

            return instance;
        }
    }

    static void Create()
    {
        instance = new DebugSettings();
        instance.Save();
    }

    public bool showObjects = true;

    void Save()
    {
#if UNITY_EDITOR
        File.WriteAllText(FILE_PATH, JsonUtility.ToJson(this, true));
#endif
    }

#if UNITY_EDITOR
    [MenuItem("Tools/Show Objects")]
    static void ShowObjects()
    {
        Instance.showObjects = true;
        Instance.Save();
    }

    [MenuItem("Tools/Hide Objects")]
    static void HideObjects()
    {
        Instance.showObjects = false;
        Instance.Save();
    }
#endif
}
}
#endif
