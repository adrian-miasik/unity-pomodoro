using AdrianMiasik.Components.Core;
using AdrianMiasik.Interfaces;
using AdrianMiasik.ScriptableObjects;
using TMPro;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.UI;

namespace AdrianMiasik.Components
{
  public class Sidebar : MonoBehaviour, IColorHook
  {
      [Header("Components")]
      [SerializeField] private BooleanToggle menuToggle;
      [SerializeField] private RectTransform container;
      [SerializeField] private Image overlayImage;
      [SerializeField] private Animation entryAnimation;
      [SerializeField] private SVGImage fill;
      [SerializeField] private SVGImage edge;
      [SerializeField] private TMP_Text versionNumber;
      
      // Components
      [Header("Sidebar Rows (Content)")]
      [SerializeField] private SidebarRow pomodoroTimer;
      [SerializeField] private SidebarRow settings;
      [SerializeField] private SidebarRow about;
      
      private PomodoroTimer timer;
      private bool isOpen;
      private Color overlay;

      public void Initialize(PomodoroTimer _timer)
      {
          timer = _timer;
          _timer.GetTheme().RegisterColorHook(this);
          ColorUpdate(_timer.GetTheme());
          
          // Initialize row components
          pomodoroTimer.Initialize(_timer, this, true);
          settings.Initialize(_timer, this);
          about.Initialize(_timer, this);
      }
      
      public void Open()
      {
          isOpen = true;
          
          container.gameObject.SetActive(true);
          gameObject.SetActive(true);
          entryAnimation.Play();
          overlayImage.enabled = true;
          
          ColorUpdate(timer.GetTheme());
          timer.ColorUpdateCreditsBubble();
      }

      public void Close()
      {
          isOpen = false;
          
          // Cancel holds in-case user holds button down and closes our menu prematurely
          pomodoroTimer.CancelHold();
          about.CancelHold();

          menuToggle.SetToFalse();
          
          container.gameObject.SetActive(false);
          entryAnimation.Stop();
          gameObject.SetActive(false);
          overlayImage.enabled = false;
          
          timer.ColorUpdateCreditsBubble();
      }

      public bool IsOpen()
      {
          return isOpen;
      }
      
      // Unity Event
      public void UpdateRows()
      {
          // Determine which row is selected
          if (timer.IsAboutPageOpen())
          {
              pomodoroTimer.Deselect();
              about.Select();
          }
          else
          {
              pomodoroTimer.Select();
              about.Deselect();
          }
      }

      public void ColorUpdate(Theme _theme)
      {
          // Overlay
          overlay = _theme.GetCurrentColorScheme().foreground;
          overlay.a = _theme.isLightModeOn ? 0.8f : 0.2f; // Following mock
          overlayImage.color = overlay;

          // Background
          fill.color = _theme.GetCurrentColorScheme().background;
          edge.color = _theme.GetCurrentColorScheme().background;
          
          // Text
          versionNumber.color = _theme.GetCurrentColorScheme().foreground;
      }
  }
}
