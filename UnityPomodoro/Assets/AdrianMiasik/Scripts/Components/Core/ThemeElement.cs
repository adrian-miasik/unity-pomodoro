using AdrianMiasik.Interfaces;
using AdrianMiasik.ScriptableObjects;
using UnityEngine;

namespace AdrianMiasik.Components.Core
{
    // TODO: Implement into most of our UP components
    public class ThemeElement: MonoBehaviour, IColorHook
    {
        protected PomodoroTimer Timer;

        /// <param name="pomodoroTimer">Main class</param>
        /// <param name="updateColors">Do you want to update or invoke the ColorUpdate method on initialization?</param>
        public virtual void Initialize(PomodoroTimer pomodoroTimer, bool updateColors = true)
        {
            Timer = pomodoroTimer;

            // Register element to theme
            pomodoroTimer.GetTheme().RegisterColorHook(this);
            
            // Update our component
            if (updateColors)
            {
                ColorUpdate(pomodoroTimer.GetTheme());
            }
        }
        
        /// <summary>
        /// Applies our theme changes to our components when necessary
        /// </summary>
        /// <param name="theme"></param>
        public virtual void ColorUpdate(Theme theme)
        {
            // Nothing
        }
    }
}