using AdrianMiasik;
using Unity.VectorGraphics;
using UnityEngine;

public class UPIcon : MonoBehaviour
{
    [SerializeField] private SVGImage icon;

    [SerializeField] private Sprite lightSprite;
    [SerializeField] private Sprite darkSprite;

    public void ColorUpdate(Theme theme)
    {
        icon.sprite = theme.isLightModeOn ? lightSprite : darkSprite;
    }
}
