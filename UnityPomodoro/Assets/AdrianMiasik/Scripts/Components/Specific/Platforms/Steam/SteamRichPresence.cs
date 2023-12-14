using System;
using System.Threading;
using AdrianMiasik.Interfaces;
using AdrianMiasik.ScriptableObjects;
using Steamworks;
using UnityEngine;

namespace AdrianMiasik.Components.Specific.Platforms.Steam
{
    // TODO: Move all SteamFriends.SetRichPresence method calls into this class. Anything that is modifying 
    // Steam Rich Presence should communicate with this class first. 
    public class SteamRichPresence : MonoBehaviour, ITimerState
    {
#if !UNITY_ANDROID && !UNITY_WSA
        private PomodoroTimer pomodoroTimer;
        private bool isInitialized;
        
        public void Initialize(PomodoroTimer timer)
        {
            pomodoroTimer = timer;
            isInitialized = true;
        }

        private void Update()
        {
            if (!isInitialized)
            {
                // Early exit
                return;
            }

            if (pomodoroTimer.IsSteamworksInitialized() && pomodoroTimer.IsSteamRichPresenceEnabled())
            {
                SteamFriends.SetRichPresence("time_left", pomodoroTimer.GetTimerString(true));
                SteamFriends.SetRichPresence("time_suffix", " left.");
            }
        }

        public void StateUpdate(PomodoroTimer.States state, Theme theme)
        {
            if (!isInitialized)
            {
                return;
            }

            if (pomodoroTimer.IsSteamworksInitialized() && pomodoroTimer.IsSteamRichPresenceEnabled())
            {
                switch (state)
                {
                    case PomodoroTimer.States.SETUP:
                        SteamFriends.SetRichPresence("steam_display", "#Setup");
                        break;
                    case PomodoroTimer.States.RUNNING:
                        SteamFriends.SetRichPresence("steam_display", "#Running");
                        break;
                    case PomodoroTimer.States.PAUSED:
                        SteamFriends.SetRichPresence("steam_display", "#Paused");
                        break;
                    case PomodoroTimer.States.COMPLETE:
                        SteamFriends.SetRichPresence("steam_display", "#Completed");
                        break;
                }
            }
        }

        public void SetRichPresence(string key, string value)
        {
            // TODO: Implement
        }
    }
#endif
}
