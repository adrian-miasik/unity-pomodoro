using Steamworks;
using UnityEngine;

namespace AdrianMiasik.Steam
{
    public class SteamManager : MonoBehaviour
    {
        private bool isInitialized;
        private const bool enableSteamworks = true;
    
        public void Initialize()
        {
            if (!enableSteamworks)
            {
                Debug.Log("Steamworks functionality disabled. (Dev)");
                return;
            }
            
            try
            {
                SteamClient.Init(2173940);
            }
            catch (System.Exception e)
            {
                Debug.Log("Unable to initialize Steam client. " + e);
            }
        
            DontDestroyOnLoad(gameObject);
            isInitialized = true;
        }
    
        private void Update()
        {
            if (isInitialized)
            {
                SteamClient.RunCallbacks();
            }
        }

        public void Shutdown()
        {
            SteamClient.Shutdown();
        }
    }
}
