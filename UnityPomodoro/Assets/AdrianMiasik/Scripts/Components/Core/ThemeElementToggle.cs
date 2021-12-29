using AdrianMiasik.Interfaces;
using AdrianMiasik.ScriptableObjects;
using UnityEngine.UI;

namespace AdrianMiasik.Components.Core
{
    public class ThemeElementToggle: Toggle, IColorHook
    {
        protected PomodoroTimer Timer;

        protected virtual void Initialize(PomodoroTimer pomodoroTimer)
        {
            Timer = pomodoroTimer;

            // Register element to theme
            pomodoroTimer.GetTheme().RegisterColorHook(this);
            
            // Update our components
            ColorUpdate(pomodoroTimer.GetTheme());
        }
        
        public virtual void ColorUpdate(Theme theme)
        {
            // Nothing
        }
    }
}