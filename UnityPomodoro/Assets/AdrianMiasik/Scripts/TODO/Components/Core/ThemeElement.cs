using AdrianMiasik.Interfaces;
using AdrianMiasik.ScriptableObjects;
using UnityEngine;

namespace AdrianMiasik.Components.Core
{
    // TODO: Implement into most of our UP components
    public class ThemeElement: MonoBehaviour, IColorHook
    {
        protected PomodoroTimer timer;
        
        protected void Initialize(PomodoroTimer _timer)
        {
            timer = _timer;

            // Register element to theme
            _timer.GetTheme().RegisterColorHook(this);
            
            // Update our components
            ColorUpdate(_timer.GetTheme());
        }
        
        public virtual void ColorUpdate(Theme _theme)
        {
            // Nothing
        }
    }
}