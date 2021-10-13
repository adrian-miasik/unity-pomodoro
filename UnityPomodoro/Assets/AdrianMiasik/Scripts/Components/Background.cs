using AdrianMiasik.Interfaces;
using AdrianMiasik.ScriptableObjects;
using UnityEngine;
using UnityEngine.UI;

namespace AdrianMiasik.Components
{
    public class Background : MonoBehaviour, IColorHook
    {
        [SerializeField] private Image background;
        [SerializeField] private Selectable selectable;

        public void Initialize(Theme _theme)
        {
            _theme.RegisterColorHook(this);
            ColorUpdate(_theme);
        }
        
        public void Select()
        {
            selectable.Select();
        }

        public void ColorUpdate(Theme _theme)
        {
            background.color = _theme.GetCurrentColorScheme().background;
        }
    }
}
