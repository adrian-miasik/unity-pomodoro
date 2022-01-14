using AdrianMiasik.Interfaces;
using AdrianMiasik.ScriptableObjects;
using UnityEngine;

namespace AdrianMiasik.Components.Base
{
    /// <summary>
    /// A base class that is intended to be used on any component/class that needs theming
    /// (that's automatically updated), and needs reference to our main class: <see cref="PomodoroTimer"/>.
    /// </summary>
    public class ThemeElement: MonoBehaviour, IColorHook
    {
        protected PomodoroTimer Timer;
        private bool isInitialized;

        /// <param name="pomodoroTimer">Our main class</param>
        /// <param name="updateColors">Do you want to update or invoke this elements ColorUpdate method
        /// on initialization?</param>
        public virtual void Initialize(PomodoroTimer pomodoroTimer, bool updateColors = true)
        {
            Timer = pomodoroTimer;

            // Register element to theme
            pomodoroTimer.GetTheme().Register(this);
            
            if (updateColors)
            {
                // Update our component
                ColorUpdate(pomodoroTimer.GetTheme());
            }

            isInitialized = true;
        }

        /// <summary>
        /// Fetches this components initialization state.
        /// </summary>
        /// <returns>Has this component been initialized yet?</returns>
        public bool IsInitialized()
        {
            return isInitialized;
        }
        
        /// <summary>
        /// Applies our theme changes to our referenced components when necessary.
        /// </summary>
        /// <param name="theme"></param>
        public virtual void ColorUpdate(Theme theme)
        {
            
        }
        
        /// <summary>
        /// De-register itself from the theme on destruction to avoid invoking <see cref="IColorHook"/>
        /// ColorUpdate() on a null reference.
        /// </summary>
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