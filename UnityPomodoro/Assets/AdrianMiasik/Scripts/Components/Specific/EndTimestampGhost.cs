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
    public class EndTimestampGhost : Ghost
    {
        protected override void Update()
        {
            base.Update();
            
            // Prevent text buttons interactions when end timer ghost is invisible OR when we are already on the 
            // main content page.
            if (m_fadeProgress <= 0 || Timer.IsMainContentOpen())
            {
                m_textContainer.blocksRaycasts = false;
            }
            else
            {
                m_textContainer.blocksRaycasts = true;
            }
        }

        public override void FadeIn(bool instantly = false)
        {
            CalculateEndTime();
            
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
            
            Color c = theme.GetCurrentColorScheme().m_foreground;
            
            // Tweak alpha indicating end timestamp quick switch interactability
            if (Timer.IsMainContentOpen())
            {
                c.a = 0.5f;
            }

            m_icon.ChangeColor(c);
        }

        /// <summary>
        /// Hooked up to the SVG icon button on the prefab.
        /// </summary>
        public void OnClick()
        {
            // If the main pomodoro page is not open...
            if (!Timer.IsMainContentOpen())
            {
                // Switch to the pomodoro timer
                Timer.ShowMainContent();
            }
        }
    }
}