using AdrianMiasik.Interfaces;
using AdrianMiasik.ScriptableObjects;
using UnityEngine.UI;

namespace AdrianMiasik.Components.Core
{
    /// <summary>
    /// A blank toggle that implements <see cref="IColorHook"/>.
    /// <remarks>Intended to be used by <see cref="BooleanToggle"/></remarks>
    /// </summary>
    public class ThemeElementToggle: Toggle, IColorHook
    {
        protected PomodoroTimer Timer;
        protected bool IsInitialized;

        protected virtual void Initialize(PomodoroTimer pomodoroTimer)
        {
            Timer = pomodoroTimer;

            // Register element to theme
            pomodoroTimer.GetTheme().Register(this);
            
            // Update our components
            ColorUpdate(pomodoroTimer.GetTheme());

            IsInitialized = true;
        }
        
        public virtual void ColorUpdate(Theme theme)
        {
            // Nothing
        }

        public new void OnDestroy()
        {
            if (IsInitialized)
            {
                Timer.GetTheme().Deregister(this);
            }
        }
    }
}