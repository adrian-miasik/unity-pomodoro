using UnityEngine;

namespace AdrianMiasik.Components.Core.Helpers
{
    public static class RectTransformHelper
    {
        public static void StretchToFit(RectTransform rectTransform)
        {
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;

            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
        }
    }
}