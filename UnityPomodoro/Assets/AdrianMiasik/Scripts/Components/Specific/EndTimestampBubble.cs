using System;
using AdrianMiasik.Components.Base;
using AdrianMiasik.ScriptableObjects;
using UnityEngine;

namespace AdrianMiasik.Components.Specific
{
    /// <summary>
    /// Used to display the end local time for the current running timer.
    /// (E.g. It's 3:02pm with 3 minutes left on the timer. Thus this will display: "3:05pm".)
    /// </summary>
    public class EndTimestampBubble : Bubble
    {
        [SerializeField] private Sprite m_workMode;
        [SerializeField] private Sprite m_breakMode;
        
        // Cache
        private float fadeProgress;
        
        public override void FadeIn(bool instantly = false)
        {
            CalculateEndTime();
            
            // Set sprite depending what the user will get prompted to do next
            if (Timer.IsOnBreak() || Timer.IsOnLongBreak())
            {
                m_icon.ChangeSprite(m_workMode);
            }
            else
            {
                m_icon.ChangeSprite(m_breakMode);
            }
            
            base.FadeIn(instantly);
        }
        
        private void CalculateEndTime()
        {
            // Get current current remaining time
            TimeSpan currentTimeSpan = TimeSpan.FromSeconds(Timer.GetCurrentTime());

            // Get system time span
            TimeSpan systemTimeSpan = DateTime.Now.TimeOfDay;
            
            // Add time spans together
            TimeSpan endTime = systemTimeSpan.Add(currentTimeSpan);
            
            // Display end time
            m_text[m_text.Count - 1].text = new DateTime(endTime.Ticks).ToLongTimeString();
        }

        public override void ColorUpdate(Theme theme)
        {
            base.ColorUpdate(theme);
            
            // Remain visible depending on theme dark mode preference
            m_icon.ChangeColor(theme.GetCurrentColorScheme().m_foreground);
        }
    }
}