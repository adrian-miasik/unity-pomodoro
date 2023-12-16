#if !UNITY_ANDROID && !UNITY_WSA
using Steamworks;
#endif
using AdrianMiasik.ScriptableObjects;
using UnityEngine;

namespace AdrianMiasik.Components.Specific.Platforms.Steam
{
    /// <summary>
    /// A class responsible for initializing our Steamworks backend.
    /// </summary>
    public class SteamManager : MonoBehaviour
    {
        private PomodoroTimer m_pomodoroTimer;
        private bool isInitialized;
        
#if !UNITY_ANDROID && !UNITY_WSA
        [SerializeField] private bool m_enableSteamworks = true;
        [SerializeField] private SteamRichPresence m_richPresence;

        public void Initialize(PomodoroTimer timer)
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
                isInitialized = false;
                return;
            }
        
            DontDestroyOnLoad(gameObject);
            m_pomodoroTimer = timer;
            isInitialized = true;
        }

        public void InitializeSteamModules()
        {
            // If steamworks is running and the presence setting is on...
            if (IsInitialized() && m_pomodoroTimer.GetSystemSettings().m_enableSteamRichPresence)
            {
                m_richPresence.Initialize(m_pomodoroTimer);
                m_pomodoroTimer.SubscribeToTimerStates(m_richPresence); // Subscribe to timer state changes
            }
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
        
        public bool IsInitialized()
        {
            return isInitialized;
        }

        // Steam Rich Presence - Piper Methods

        public void UpdateState(PomodoroTimer.States state, Theme theme)
        {
            m_richPresence.StateUpdate(state, theme);
        }

        public bool IsRichPresenceInitialized()
        {
            return m_richPresence.IsInitialized();
        }

        public void SetRichPresence(string key, string value)
        {
            m_richPresence.SetRichPresence(key, value);
        }

        public void ClearSteamRichPresence()
        {
            m_richPresence.Clear();
        }
    }
}
