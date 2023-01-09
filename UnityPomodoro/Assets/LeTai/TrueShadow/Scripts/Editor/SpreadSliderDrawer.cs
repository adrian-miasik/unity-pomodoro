using UnityEditor;
using UnityEngine;
using static UnityEditor.EditorGUI;

namespace LeTai.TrueShadow.Editor
{
[CustomPropertyDrawer(typeof(SpreadSliderAttribute))]
public class SpreadSliderDrawer : PropertyDrawer
{
    const float SLIDER_SPACING = 5;
    const float MARKER_HEIGHT  = 6;
    const float MARKER_ALPHA   = .75f;
    const float MARKER_FILLET  = 2;

    static readonly Vector4 START_RADII = new Vector4(MARKER_FILLET, 0,             0,             MARKER_FILLET);
    static readonly Vector4 END_RADII   = new Vector4(0,             MARKER_FILLET, MARKER_FILLET, 0);

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        using (var propScope = new PropertyScope(position, label, property))
        using (var changeScope = new ChangeCheckScope())
        {
            var controlPosition = PrefixLabel(position, propScope.content);

            var floatFieldWidth = Mathf.Min(EditorGUIUtility.fieldWidth, controlPosition.width);
            var sliderPosition = new Rect(controlPosition)
                {width = controlPosition.width - floatFieldWidth - SLIDER_SPACING};

            const float marker1 = .8f;
            const float marker2 = .95f;

            DrawMarkers(sliderPosition,
                        (marker1, new Color(1.00000f, 0.60392f, 0.01961f, MARKER_ALPHA)),
                        (marker2, new Color(1.00000f, 0.25490f, 0.20784f, MARKER_ALPHA)));

            var newVal = Slider(controlPosition,
                                GUIContent.none,
                                property.floatValue,
                                0, 1);

            if (!Event.current.control && !Event.current.alt)
            {
                var dist1 = (newVal - marker1) * sliderPosition.width;
                var dist2 = (newVal - marker2) * sliderPosition.width;
                if (0 < dist1 && dist1 < 4)
                    newVal = marker1;
                if (0 < dist2 && dist2 < 4)
                    newVal = marker2;
            }

            if (changeScope.changed)
                property.floatValue = newVal;
        }
    }

    void DrawMarkers(Rect sliderPosition, params (float, Color)[] markers)
    {
        var hPad         = GUI.skin.horizontalSliderThumb.fixedWidth / 2f;
        var markerXStart = sliderPosition.x + hPad;
        var markerXEnd   = sliderPosition.width - hPad * 2;
        var vPad         = (sliderPosition.height - MARKER_HEIGHT) / 2f;
        var markerYStart = sliderPosition.y + vPad;
        var markerHeight = sliderPosition.height - vPad * 2;
        for (var i = 0; i < markers.Length; i++)
        {
            var (offset, color) = markers[i];

            var x = markerXStart + markerXEnd * offset;
            var width = i < markers.Length - 1
                            ? sliderPosition.width * (markers[i + 1].Item1 - offset) - 1
                            : sliderPosition.xMax - x;
            var position = new Rect {
                x      = x,
                y      = markerYStart,
                width  = width,
                height = markerHeight
            };

            var radii = i == 0 ? START_RADII : END_RADII;

            GUI.DrawTexture(position,
                            Texture2D.whiteTexture, ScaleMode.StretchToFill, true, 0, color,
                            Vector4.zero, radii);
        }
    }
}
}
