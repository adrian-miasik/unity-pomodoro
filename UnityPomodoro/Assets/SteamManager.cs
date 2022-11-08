using Steamworks;
using UnityEngine;

public class SteamManager : MonoBehaviour
{
    private bool isInitialized;
    
    public void Initialize()
    {
        try
        {
            SteamClient.Init(2173940);
        }
        catch (System.Exception e)
        {
            Debug.Log("Unable to initialize Steam client.");
        }
        
        DontDestroyOnLoad(gameObject);
        isInitialized = true;

        Debug.Log("Steam User Found: " + SteamClient.Name);
    }
    
    private void Update()
    {
        if (isInitialized)
        {
            SteamClient.RunCallbacks();
        }
    }
}
