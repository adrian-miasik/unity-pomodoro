using AdrianMiasik.Interfaces;
using AdrianMiasik.ScriptableObjects;
using UnityEngine;

namespace AdrianMiasik.Components.Core
{
    // TODO: Implement into most of our UP components
    public class ThemeElement: MonoBehaviour, IColorHook
    {
        protected PomodoroTimer Timer;
        
        public void Initialize(PomodoroTimer pomodoroTimer)
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