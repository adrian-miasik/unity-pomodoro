using AdrianMiasik.Interfaces;
using UnityEngine;
using UnityEngine.UI;

namespace AdrianMiasik.Components
{
    public class Background : MonoBehaviour, IColorHook
    {
        [SerializeField] private Image background;
        [SerializeField] private Selectable selectable;

        public void Initialize(Theme theme)
        {
            theme.RegisterColorHook(this);
            ColorUpdate(theme.GetCurrentColorScheme());
        }
        
        public void Select()
        {
            selectable.Select();
        }

        public void ColorUpdate(ColorScheme currentColors)
        {
            background.color = currentColors.background;
        }
    }
}
