using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using static LeTai.TrueShadow.Math;
using EGU = UnityEditor.EditorGUIUtility;

namespace LeTai.TrueShadow.Editor
{
[CustomPropertyDrawer(typeof(KnobAttribute))]
public class KnobPropertyDrawer : PropertyDrawer
{
    public static bool procrastinationMode = false;

    static readonly Texture2D KNOB_BG_TEXTURE = Utility.FindEditorResource<Texture2D>("Knob_BG");
    static readonly Texture2D KNOB_FG_TEXTURE = Utility.FindEditorResource<Texture2D>("Knob_FG");

    static readonly MethodInfo DO_FLOAT_FIELD_METHOD;
    static readonly FieldInfo  RECYCLED_EDITOR_PROPERTY;
    static readonly FieldInfo  FLOAT_FIELD_FORMAT_STRING_CONST;

    static readonly Color KNOB_BG_COLOR;
    static readonly Color KNOB_FG_COLOR;
    static readonly Color KNOB_FG_COLOR_ACTIVE;

    static KnobPropertyDrawer()
    {
        var editorGUIType = typeof(EditorGUI);

        const BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Static;

        Type[] argumentTypes = {
            Assembly.GetAssembly(editorGUIType).GetType("UnityEditor.EditorGUI+RecycledTextEditor"),
            typeof(Rect),
            typeof(Rect),
            typeof(int),
            typeof(float),
            typeof(string),
            typeof(GUIStyle),
            typeof(bool)
        };
        DO_FLOAT_FIELD_METHOD           = editorGUIType.GetMethod("DoFloatField", flags, null, argumentTypes, null);
        RECYCLED_EDITOR_PROPERTY        = editorGUIType.GetField("s_RecycledEditor",        flags);
        FLOAT_FIELD_FORMAT_STRING_CONST = editorGUIType.GetField("kFloatFieldFormatString", flags);

        if (EGU.isProSkin)
        {
            KNOB_BG_COLOR        = new Color(.164f, .164f, .164f);
            KNOB_FG_COLOR        = new Color(.701f, .701f, .701f);
            KNOB_FG_COLOR_ACTIVE = new Color(.49f,  .67f,  .94f);
        }
        else
        {
            KNOB_BG_COLOR        = new Color(.941f, .941f, .941f);
            KNOB_FG_COLOR        = new Color(.239f, .239f, .239f);
            KNOB_FG_COLOR_ACTIVE = new Color(.054f, .274f, .549f);
        }
    }

    static float DoFloatFieldInternal(Rect     position,
                                      Rect     dragHotZone,
                                      int      id,
                                      float    value,
                                      string   formatString = null,
                                      GUIStyle style        = null,
                                      bool     draggable    = true)
    {
        style        = style ?? EditorStyles.numberField;
        formatString = formatString ?? (string) FLOAT_FIELD_FORMAT_STRING_CONST.GetValue(null);

        var editor = RECYCLED_EDITOR_PROPERTY.GetValue(null);

        return (float) DO_FLOAT_FIELD_METHOD.Invoke(null, new[] {
            editor,
            position,
            dragHotZone,
            id,
            value,
            formatString,
            style,
            draggable
        });
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (!(attribute is KnobAttribute)) return;

        KnobProperty(position, label, property, Vector2.right);
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return ControlHeight;
    }

    static float ControlHeight => EGU.singleLineHeight * 2.0f;
    static float KnobSize      => EGU.singleLineHeight * 2.5f;
    static float KnobYOffset   => (ControlHeight - KnobSize) / 2;

    static Color Lighten(Color color, float amount)
    {
        Color.RGBToHSV(color, out var h, out var s, out var v);
        return Color.HSVToRGB(h, s, v + amount);
    }

    public static void KnobProperty(Rect rect, GUIContent label, SerializedProperty prop, Vector2 zeroVector)
    {
        float angle     = prop.floatValue;
        float prevAngle = angle;

        using (var propScope = new EditorGUI.PropertyScope(rect, label, prop))
        using (var changeScope = new EditorGUI.ChangeCheckScope())
        {
            var labelRect = new Rect(rect) {
                y      = rect.y + (ControlHeight - EGU.singleLineHeight) / 2,
                height = EGU.singleLineHeight
            };

            int fieldId   = GUIUtility.GetControlID(FocusType.Keyboard, labelRect);
            var fieldRect = EditorGUI.PrefixLabel(labelRect, fieldId, propScope.content);
            labelRect.xMax  =  fieldRect.x;
            fieldRect.x     += ControlHeight;
            fieldRect.width -= ControlHeight;


            Rect knobRect = new Rect(rect.x + EGU.labelWidth + KnobYOffset,
                                     rect.y + KnobYOffset,
                                     KnobSize, KnobSize);
            int knobId = GUIUtility.GetControlID(FocusType.Passive, knobRect);

            if (Event.current != null)
            {
                if (Event.current.type == EventType.MouseDown && knobRect.Contains(Event.current.mousePosition))
                {
                    GUIUtility.hotControl = knobId;

                    angle = Angle360(zeroVector, Event.current.mousePosition - knobRect.center);
                }
                else if (Event.current.type == EventType.MouseUp && GUIUtility.hotControl == knobId)
                {
                    GUIUtility.hotControl = 0;
                }
                else if (Event.current.type == EventType.MouseDrag && GUIUtility.hotControl == knobId)
                {
                    angle = Angle360(zeroVector, Event.current.mousePosition - knobRect.center);
                }
                else if (Event.current.type == EventType.Repaint)
                {
                    var notRotated = GUI.matrix;
                    var oldColor   = GUI.color;
                    var highlighted = GUIUtility.hotControl == knobId ||
                                      GUIUtility.hotControl == fieldId ||
                                      GUIUtility.keyboardControl == fieldId;

                    GUIUtility.RotateAroundPivot(angle, knobRect.center);
                    GUI.color = KNOB_BG_COLOR;
                    GUI.DrawTexture(knobRect, KNOB_BG_TEXTURE, ScaleMode.ScaleToFit, true, 1);
                    GUI.color = highlighted ? KNOB_FG_COLOR_ACTIVE : KNOB_FG_COLOR;
                    if (procrastinationMode) GUI.color = Color.red;
                    GUI.DrawTexture(knobRect, KNOB_FG_TEXTURE, ScaleMode.ScaleToFit, true, 1);

                    if (!procrastinationMode)
                        GUI.matrix = notRotated;
                    GUI.color = oldColor;
                }

                if (angle != prevAngle) GUI.changed = true;
            }


            angle = DoFloatFieldInternal(
                fieldRect,
                labelRect,
                fieldId,
                angle
            );


            if (changeScope.changed) prop.floatValue = angle;
        }
    }
}
}
