using AdrianMiasik.ScriptableObjects;
using Unity.VectorGraphics;
using UnityEngine;

namespace AdrianMiasik.Components
{
    public class UPIcon : MonoBehaviour
    {
        [SerializeField] private SVGImage icon;

        [SerializeField] private Sprite lightSprite;
        [SerializeField] private Sprite darkSprite;

        public void ColorUpdate(Theme _theme)
        {
            icon.sprite = _theme.isLightModeOn ? lightSprite : darkSprite;
        }
    }
}
