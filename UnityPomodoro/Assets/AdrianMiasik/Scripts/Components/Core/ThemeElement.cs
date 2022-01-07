using AdrianMiasik.Interfaces;
using AdrianMiasik.ScriptableObjects;
using UnityEngine;

namespace AdrianMiasik.Components.Core
{
    // TODO: Implement into most of our UP components
    public class ThemeElement: MonoBehaviour, IColorHook
    {
        protected PomodoroTimer Timer;
        private bool isInitialized;

        /// <param name="pomodoroTimer">Main class</param>
        /// <param name="updateColors">Do you want to update or invoke the ColorUpdate method on initialization?</param>
        public virtual void Initialize(PomodoroTimer pomodoroTimer, bool updateColors = true)
        {
            Timer = pomodoroTimer;

            // Register element to theme
            pomodoroTimer.GetTheme().RegisterColorHook(this);
            
            if (updateColors)
            {
                // Update our component
                ColorUpdate(pomodoroTimer.GetTheme());
            }

            isInitialized = true;
        }

        public bool IsInitialized()
        {
            return isInitialized;
        }
        
        /// <summary>
        /// Applies our theme changes to our components when necessary
        /// </summary>
        /// <param name="theme"></param>
        public virtual void ColorUpdate(Theme theme)
        {
            // Nothing
        }

        public void OnDestroy()
        {
            if (isInitialized)
            {
                Debug.Log("Theme element is being deregistered.");
                Timer.GetTheme().Deregister(this);
            }
        }
    }
}