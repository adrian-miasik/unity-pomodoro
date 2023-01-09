using System;
using System.Collections.Generic;
using UnityEngine;

namespace LeTai.TrueShadow
{
public class ProjectSettings : ScriptableObject
{
    public const string DEFAULT_RESOURCE_PATH = "True Shadow Project Settings Default";

#if LETAI_TRUESHADOW_DEBUG
    public const string RESOURCE_PATH = DEFAULT_RESOURCE_PATH;
#else
    public const string RESOURCE_PATH = "True Shadow Project Settings";
#endif

    static ProjectSettings instance;

    public static ProjectSettings Instance
    {
        get
        {
            if (!instance) instance = Resources.Load<ProjectSettings>(RESOURCE_PATH);
            if (!instance)
            {
                Debug.LogError($"Can't find \"{RESOURCE_PATH}\" in a Resources folder. Please restore the file or re-install True Shadow.");
            }

            return instance;
        }
    }

    [SerializeField] internal bool useGlobalAngleByDefault = false;

    [Knob]
    [SerializeField] internal float globalAngle = 90f;

    [SerializeField] internal bool showQuickPresetsButtons = true;

    [SerializeField] internal List<QuickPreset> quickPresets = new List<QuickPreset>(8);

    public bool UseGlobalAngleByDefault
    {
        get => useGlobalAngleByDefault;
        private set => useGlobalAngleByDefault = value;
    }

    public float GlobalAngle
    {
        get => globalAngle;
        private set
        {
            globalAngle = value;
            globalAngleChanged?.Invoke(globalAngle);
        }
    }

    public bool ShowQuickPresetsButtons
    {
        get => showQuickPresetsButtons;
        private set => showQuickPresetsButtons = value;
    }

    public List<QuickPreset> QuickPresets
    {
        get => quickPresets;
        private set => quickPresets = value;
    }

    public event Action<float> globalAngleChanged;
}
}
