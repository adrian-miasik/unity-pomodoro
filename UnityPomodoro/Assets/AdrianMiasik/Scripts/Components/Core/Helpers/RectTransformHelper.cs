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

        public static void ApplyRectTransform(RectTransform sourceToCopy, RectTransform targetToModify)
        {
            targetToModify.anchoredPosition = sourceToCopy.anchoredPosition;
            targetToModify.anchorMax = sourceToCopy.anchorMax;
            targetToModify.anchorMin = sourceToCopy.anchorMin;
            targetToModify.offsetMin = sourceToCopy.offsetMin;
            targetToModify.offsetMax = sourceToCopy.offsetMax;
            targetToModify.pivot = sourceToCopy.pivot;
        }
    }
}