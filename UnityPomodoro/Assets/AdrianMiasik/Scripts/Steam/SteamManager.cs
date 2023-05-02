#if !UNITY_ANDROID && !UNITY_WSA
using Steamworks;
#endif
using UnityEngine;

namespace AdrianMiasik.Steam
{
    /// <summary>
    /// A class responsible for initializing our Steamworks backend.
    /// </summary>
    public class SteamManager : MonoBehaviour
    {
#if !UNITY_ANDROID && !UNITY_WSA
        private bool isInitialized;
        [SerializeField] private bool m_enableSteamworks = true;
    
        public void Initialize()
        {
            if (!m_enableSteamworks)
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
#endif
    }
}
