using AdrianMiasik.Components.Base;
using AdrianMiasik.Components.Core;
using AdrianMiasik.Interfaces;
using AdrianMiasik.ScriptableObjects;
using Steamworks;
using Steamworks.Data;
using UnityEngine;

namespace AdrianMiasik.Components.Specific
{
    /// <summary>
    /// A <see cref="ThemeElement"/> button used for skipping / auto-completing the timer. Implements
    /// <see cref="ITimerState"/> to show / hide itself depending if the timer has been started / not started.
    /// </summary>
    public class SkipButton : ThemeElement, ITimerState
    {
        [SerializeField] private ClickButtonSVGIcon m_buttonSvg;

        public override void Initialize(PomodoroTimer pomodoroTimer, bool updateColors = true)
        {
            base.Initialize(pomodoroTimer, updateColors);
            m_buttonSvg.m_onClick.AddListener(Skip);
            Hide();
        }

        public void StateUpdate(PomodoroTimer.States state, Theme theme)
        {
            if (state == PomodoroTimer.States.RUNNING || state == PomodoroTimer.States.PAUSED)
            {
                Show();
            }
            else
            {
                Hide();
            }
        }

        public override void ColorUpdate(Theme theme)
        {
            base.ColorUpdate(theme);
            m_buttonSvg.m_icon.color = theme.GetCurrentColorScheme().m_foreground;
        }

        private void Show()
        {
            gameObject.SetActive(true);
        }

        private void Hide()
        {
            gameObject.SetActive(false);
        }

        private void Skip()
        {
            AudioMimic.Instance.PlaySound(m_buttonSvg.m_clickSound.clip);
            Timer.Skip();
            
            // Check if steam client is found...
            if (SteamClient.IsValid)
            {
                // Fetch skip achievement
                Achievement ach = new("ACH_KETCHUP");
                
                // If achievement is not unlocked...
                if (!ach.State)
                {
                    ach.Trigger();
                    Debug.Log("Steam Achievement Unlocked! 'Ketchup: Skip your first timer.'");
                }
            }
        }
    }
}
