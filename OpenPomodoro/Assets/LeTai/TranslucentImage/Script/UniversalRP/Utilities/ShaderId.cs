using UnityEngine;

namespace LeTai.Asset.TranslucentImage.UniversalRP
{
public static class ShaderId
{
    public static readonly int MAIN_TEX = Shader.PropertyToID("_MainTex");

    private static bool  isInitialized;
    public static  int[] intermediateRT;


    public static void Init(int stackDepth)
    {
        if (isInitialized)
            return;

        intermediateRT = new int[stackDepth * 2 - 1];
        for (var i = 0; i < intermediateRT.Length; i++)
        {
            intermediateRT[i] = Shader.PropertyToID($"TI_intermediate_rt_{i}");
        }

        isInitialized = true;
    }
}
}
