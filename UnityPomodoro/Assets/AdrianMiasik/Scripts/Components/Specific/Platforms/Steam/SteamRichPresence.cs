using System;
using AdrianMiasik.Interfaces;
using AdrianMiasik.ScriptableObjects;
using Steamworks;
using UnityEngine;

namespace AdrianMiasik.Components.Specific.Platforms.Steam
{
    public class SteamRichPresence : MonoBehaviour, ITimerState
    {
#if !UNITY_ANDROID && !UNITY_WSA
        private bool isInitialized;
        
        public void Initialize()
        {
            isInitialized = true;
        }
        
        public void StateUpdate(PomodoroTimer.States state, Theme theme)
        {
            if (!isInitialized)
            {
                return;
            }
            
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
                    SteamFriends.ClearRichPresence();
                    break;
            }
            
        }
    }
#endif
}
