using AdrianMiasik.Components.Core;
using AdrianMiasik.Interfaces;
using AdrianMiasik.ScriptableObjects;
using UnityEngine;
using UnityEngine.UI;

namespace AdrianMiasik.Components
{
  public class Sidebar : MonoBehaviour, IColorHook
  {
      [SerializeField] private BooleanToggle menuToggle;
      [SerializeField] private RectTransform container;
      [SerializeField] private Image overlayImage;
      [SerializeField] private Animation entryAnimation;

      private PomodoroTimer timer;
      private Color overlay;

      public void Initialize(PomodoroTimer _timer)
      {
          timer = _timer;
          _timer.GetTheme().RegisterColorHook(this);
          ColorUpdate(_timer.GetTheme());
      }
      
      public void Open()
      {
          container.gameObject.SetActive(true);
          gameObject.SetActive(true);
          entryAnimation.Play();
          overlayImage.enabled = true;
          ColorUpdate(timer.GetTheme());
      }

      public void Close()
      {
          menuToggle.SetToFalse();
          container.gameObject.SetActive(false);
          entryAnimation.Stop();
          gameObject.SetActive(false);
          overlayImage.enabled = false;
      }

      public void ColorUpdate(Theme _theme)
      {
          overlay = _theme.GetCurrentColorScheme().foreground;
          overlay.a = _theme.isLightModeOn ? 0.8f : 0.2f; // Following mock
          overlayImage.color = overlay;
      }
  }
}
