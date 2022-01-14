using AdrianMiasik.Components.Core;
using AdrianMiasik.Interfaces;
using AdrianMiasik.ScriptableObjects;

namespace AdrianMiasik.Components.Base
{
    /// <summary>
    /// A **base** (blank) toggle that implements <see cref="IColorHook"/> (See <see cref="ToggleSprite"/> logic).
    /// <remarks>Intended to be used by <see cref="BooleanSlider"/> and <see cref="ThemeElement"/></remarks>
    /// </summary>
    public class Toggle: UnityEngine.UI.Toggle, IColorHook
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