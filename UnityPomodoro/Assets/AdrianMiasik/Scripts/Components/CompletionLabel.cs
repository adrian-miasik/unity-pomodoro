using System.Collections.Generic;
using AdrianMiasik.Interfaces;
using AdrianMiasik.ScriptableObjects;
using TMPro;
using UnityEngine;

namespace AdrianMiasik.Components
{
    public class CompletionLabel : MonoBehaviour, IColorHook
    {
        [SerializeField] private List<TMP_Text> labels = new List<TMP_Text>();

        public void Initialize(Theme _theme)
        {
            _theme.RegisterColorHook(this);
            ColorUpdate(_theme);
        }

        public void ColorUpdate(Theme _theme)
        {
            foreach (TMP_Text _text in labels)
            {
                _text.color = _theme.GetCurrentColorScheme().complete;
            }
        }
    }
}
