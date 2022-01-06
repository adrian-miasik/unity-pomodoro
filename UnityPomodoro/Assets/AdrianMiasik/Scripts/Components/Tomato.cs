using AdrianMiasik.Components.Core;
using AdrianMiasik.ScriptableObjects;

namespace AdrianMiasik.Components
{
    public class Tomato : ThemeElement
    {
        public override void Initialize(PomodoroTimer pomodoroTimer, bool updateColors = true)
        {
            base.Initialize(pomodoroTimer, updateColors);
        }

        public override void ColorUpdate(Theme theme)
        {
            base.ColorUpdate(theme);
        }
    }
}
