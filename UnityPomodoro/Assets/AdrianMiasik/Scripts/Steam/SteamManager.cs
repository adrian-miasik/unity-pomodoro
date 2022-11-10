using Steamworks;
using UnityEngine;

namespace AdrianMiasik.Steam
{
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
                Debug.Log("Unable to initialize Steam client. " + e);
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
}
