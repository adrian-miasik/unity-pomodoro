using System;
using System.Collections.Generic;
using AdrianMiasik.Components.Core;
using AdrianMiasik.Interfaces;
using AdrianMiasik.ScriptableObjects;
using UnityEngine;
using UnityEngine.UI;

namespace AdrianMiasik.Components
{
    public class DigitFormat : ThemeElement
    {
        public enum Digits
        {
            DAYS,
            HOURS,
            MINUTES,
            SECONDS,
            MILLISECONDS
        }

        public enum SupportedFormats
        {
            DD_HH_MM_SS_MS, // Full
            HH_MM_SS_MS,    // Detail
            HH_MM_SS,       // Default / Standard
            MM_SS,          // Simple
            SS              // Bare
        }

        // Current format
        [SerializeField] private RectTransform m_self; // Height modifier
        [SerializeField] private RectTransform m_digitFormatRect; // Width modifier
        [SerializeField] private SupportedFormats m_format;
        
        [Header("Source Prefabs")]
        [SerializeField] private DoubleDigit m_digitSource;
        [SerializeField] private DigitSeparator m_separatorSource;
        
        [Header("Work Data")]
        [SerializeField] private int[] m_workTime = {0,0,25,0,0}; // Represents data for DD_HH_MM_SS_MS

        [Header("Break Data")]
        public bool m_isOnBreak;
        [SerializeField] private int[] m_breakTime = {0,0,5,0,0}; // Represents data for DD_HH_MM_SS_MS

        // Cache
        private List<DoubleDigit> generatedDigits;
        private List<DigitSeparator> generatedSeparators;
        private int previousFormatSelection = -1;

        private void OnValidate()
        {
            // Prevent values from going over their limit
            if (!m_isOnBreak)
            {
                for (int i = 0; i < m_workTime.Length; i++)
                {
                    m_workTime[i] = Mathf.Clamp(m_workTime[i], GetDigitMin(), GetDigitMax((Digits)i));
                }
            }
            else
            {
                for (int i = 0; i < m_breakTime.Length; i++)
                {
                    m_breakTime[i] = Mathf.Clamp(m_breakTime[i], GetDigitMin(), GetDigitMax((Digits)i));
                }
            }
        }

        public override void Initialize(PomodoroTimer pomodoroTimer, bool updateColors = true)
        {
            base.Initialize(pomodoroTimer, false);
             
            SwitchFormat(m_format);
            GenerateFormat();
            
            ColorUpdate(Timer.GetTheme());
        }

        [ContextMenu("Generate Format")]
        public void GenerateFormat()
        {
            generatedDigits?.Clear();
            generatedSeparators?.Clear();

            // Clear pre-generated digits / transforms from pre-built scene
            foreach (Transform t in m_digitFormatRect.GetComponentsInChildren<Transform>())
            {
                if (t == m_digitFormatRect.transform)
                {
                    continue;
                }

                // TODO: Improve performance / remove get component call, possibly with a theme manager class?
                // Deregister color hook
                foreach (IColorHook colorHook in t.GetComponentsInChildren<IColorHook>())
                {
                    Timer.GetTheme().Deregister(colorHook);
                }
                
                Destroy(t.gameObject);
            }

            // Generate our digits and separators (format)
            char[] separatorChar = { '_' };
            GenerateFormat(GetDoubleDigitSet(m_format, separatorChar[0]));

            // Improve visuals based on number of generated elements
            ImproveLayoutVisuals();

            // Calculate time
            SetTime(GetTime());
                
            // Apply
            RefreshDigitVisuals();

            ColorUpdate(Timer.GetTheme());
        }

        public void RefreshDigitVisuals()
        {
            foreach (DoubleDigit digit in generatedDigits)
            {
                digit.UpdateVisuals(GetDigitValue(digit.m_digit));
                digit.HideArrows();
            }
        }
        
        // Specific to our anchored layouts
        private void ImproveLayoutVisuals()
        {
            switch (generatedDigits.Count)
            {
                case 1:
                    // Width
                    m_digitFormatRect.anchorMin = new Vector2(0.38f, m_digitFormatRect.anchorMin.y);
                    m_digitFormatRect.anchorMax = new Vector2(0.62f, m_digitFormatRect.anchorMax.y);
                    m_digitFormatRect.anchoredPosition = Vector2.zero;
                    
                    // Height
                    ResetHeight();
                    
                    break;
                
                case 2:
                    // Width
                    m_digitFormatRect.anchorMin = new Vector2(0.225f, m_digitFormatRect.anchorMin.y);
                    m_digitFormatRect.anchorMax = new Vector2(0.775f, m_digitFormatRect.anchorMax.y);
                    m_digitFormatRect.anchoredPosition = Vector2.zero;
                    
                    // Height
                    ResetHeight();
                    
                    break;
                
                case 4:
                    // Width
                    ResetWidth();
                    
                    // Height
                    m_self.anchorMin = new Vector2(m_self.anchorMin.x, 0.4f);
                    m_self.anchorMax = new Vector2(m_self.anchorMax.x, 0.7f);
                    m_self.anchoredPosition = Vector2.zero;
                    
                    break;
                
                case 5:
                    // Width
                    ResetWidth();
                    
                    // Height
                    m_self.anchorMin = new Vector2(m_self.anchorMin.x, 0.425f);
                    m_self.anchorMax = new Vector2(m_self.anchorMax.x, 0.675f);
                    m_self.anchoredPosition = Vector2.zero;
                    
                    break;
                
                default:
                    ResetWidth();
                    ResetHeight();
                    
                    break;
            }
        }

        private void ResetWidth()
        {
            // Reset Width
            m_digitFormatRect.anchorMin = new Vector2(0.05f, m_digitFormatRect.anchorMin.y);
            m_digitFormatRect.anchorMax = new Vector2(0.95f, m_digitFormatRect.anchorMax.y);
            m_digitFormatRect.anchoredPosition = Vector2.zero;
        }

        private void ResetHeight()
        {
            // Reset Height
            m_self.anchorMin = new Vector2(m_self.anchorMin.x, 0.35f);
            m_self.anchorMax = new Vector2(m_self.anchorMax.x, 0.75f);
            m_self.anchoredPosition = Vector2.zero;
        }
        
        public void FlipIsOnBreakBool()
        {
            m_isOnBreak = !m_isOnBreak;
        }

        /// <summary>
        /// Returns the users own set time that has been set during timer setup.
        /// </summary>
        /// <returns></returns>
        public TimeSpan GetTime()
        {
            TimeSpan ts;
            if (!m_isOnBreak)
            {
                ts = TimeSpan.FromDays(m_workTime[0]) +
                      TimeSpan.FromHours(m_workTime[1]) +
                      TimeSpan.FromMinutes(m_workTime[2]) +
                      TimeSpan.FromSeconds(m_workTime[3]) +
                      TimeSpan.FromMilliseconds(m_workTime[4]);
            }
            else
            {
                ts = TimeSpan.FromDays(m_breakTime[0]) +
                      TimeSpan.FromHours(m_breakTime[1]) +
                      TimeSpan.FromMinutes(m_breakTime[2]) +
                      TimeSpan.FromSeconds(m_breakTime[3]) +
                      TimeSpan.FromMilliseconds(m_breakTime[4]);
            }

            return ts;
        }

        /// <summary>
        /// Returns a list of key value pairs using the provided enum format.
        /// Each key string determines it's type (such as hour/minute/second/etc...) and,
        /// each value int determines it's starting value.
        /// </summary>
        /// <param name="format"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        private List<KeyValuePair<string, int>> GetDoubleDigitSet(SupportedFormats format, char separator)
        {
            string formatString = format.ToString();
            string currentAbbreviation = String.Empty;

            List<KeyValuePair<string, int>> result = new List<KeyValuePair<string, int>>();

            // Iterate through each character in enum
            for (int i = 0; i < formatString.Length; i++)
            {
                char currentCharacter = formatString[i];

                // If we hit our separator...
                if (currentCharacter == separator)
                {
                    // Cache our abbreviation and value
                    result.Add(new KeyValuePair<string, int>(currentAbbreviation, 0)); // Starting value of zero
                    currentAbbreviation = String.Empty;
                }
                else
                {
                    currentAbbreviation += currentCharacter;

                    // If we reached the end of our list...
                    if (i == formatString.Length - 1)
                    {
                        result.Add(new KeyValuePair<string, int>(currentAbbreviation, 0)); // Starting value of zero
                    }
                }
            }

            // Return abbreviation and value set
            return result;
        }

        /// <summary>
        /// Generates the appropriate amount of digits and separators based on the provided data set
        /// </summary>
        /// <param name="doubleDigitSetToGenerate">String represents abbreviation, and int represents value</param>
        private void GenerateFormat(List<KeyValuePair<string, int>> doubleDigitSetToGenerate)
        {
            generatedDigits = new List<DoubleDigit>();
            generatedSeparators = new List<DigitSeparator>();

            for (int i = 0; i < doubleDigitSetToGenerate.Count; i++)
            {
                KeyValuePair<string, int> pair = doubleDigitSetToGenerate[i];

                // Generate double digit
                DoubleDigit dd = Instantiate(m_digitSource, m_digitFormatRect);
                dd.Initialize(Timer, this, GetDigitType(pair.Key));
                generatedDigits.Add(dd);
                
                // Skip last iteration to avoid spacer generation
                if (i == doubleDigitSetToGenerate.Count - 1)
                {
                    break;
                }

                // Generate spacer (between each character)
                DigitSeparator separator = Instantiate(m_separatorSource, m_digitFormatRect);
                generatedSeparators.Add(separator);
            }

            // Hook up selectable navigation's
            for (int i = 0; i < generatedDigits.Count; i++)
            {
                DoubleDigit digit = generatedDigits[i];

                // Create navigation
                Navigation digitNav = new Navigation
                {
                    mode = Navigation.Mode.Explicit,
                    selectOnLeft = generatedDigits[Wrap(i - 1, generatedDigits.Count)].GetSelectable(),
                    selectOnRight = generatedDigits[Wrap(i + 1, generatedDigits.Count)].GetSelectable()
                };

                // Apply navigation
                digit.GetSelectable().navigation = digitNav;
            }

            // Fix background navigation
            Navigation backgroundNav = new Navigation
            {
                mode = Navigation.Mode.Explicit,
                selectOnRight = generatedDigits[0].GetSelectable(),
                selectOnLeft = generatedDigits[generatedDigits.Count - 1].GetSelectable()
            };
            Timer.SetBackgroundNavigation(backgroundNav);
        }

        private int Wrap(int index, int length)
        {
            return (index % length + length) % length;
        }

        public void ShowTime(TimeSpan ts)
        {
            foreach (DoubleDigit doubleDigit in generatedDigits)
            {
                switch (doubleDigit.m_digit)
                {
                    case Digits.DAYS:
                        doubleDigit.SetTextLabel(ts.Days);
                        break;
                    case Digits.HOURS:
                        doubleDigit.SetTextLabel(ts.Hours);
                        break;
                    case Digits.MINUTES:
                        doubleDigit.SetTextLabel(ts.Minutes);
                        break;
                    case Digits.SECONDS:
                        doubleDigit.SetTextLabel(ts.Seconds);
                        break;
                    case Digits.MILLISECONDS:
                        string millisecondsString = Mathf.Abs(ts.Milliseconds).ToString();
                        if (millisecondsString.Length >= 2)
                        {
                            doubleDigit.SetTextLabel(
                                    int.Parse(millisecondsString.Remove(millisecondsString.Length - 1, 1)));
                        }
                        else
                        {
                            doubleDigit.SetTextLabel(ts.Milliseconds);
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public void SetTime(TimeSpan ts)
        {
            // Clear out time arrays
            if (!m_isOnBreak)
            {
                Array.Clear(m_workTime, 0, m_workTime.Length);
            }
            else
            {
                Array.Clear(m_breakTime, 0, m_breakTime.Length);
            }

            // Apply time arrays to update only for generated digits (essentially throwing out data that's not used)
            foreach (DoubleDigit doubleDigit in generatedDigits)
            {
                Digits type = doubleDigit.m_digit;

                switch (type)
                {
                    case Digits.DAYS:
                        doubleDigit.SetValue(ts.Days);
                        break;
                    case Digits.HOURS:
                        doubleDigit.SetValue(ts.Hours);
                        break;
                    case Digits.MINUTES:
                        doubleDigit.SetValue(ts.Minutes);
                        break;
                    case Digits.SECONDS:
                        doubleDigit.SetValue(ts.Seconds);
                        break;
                    case Digits.MILLISECONDS:
                        doubleDigit.SetValue(ts.Milliseconds);
                        break;
                }
            }
        }

        /// <summary>
        /// Returns the digit type using the provided abbreviation string
        /// </summary>
        /// <param name="abbreviation"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private static Digits GetDigitType(string abbreviation)
        {
            switch (abbreviation)
            {
                case "DD":
                    return Digits.DAYS;
                case "HH":
                    return Digits.HOURS;
                case "MM":
                    return Digits.MINUTES;
                case "SS":
                    return Digits.SECONDS;
                case "MS":
                    return Digits.MILLISECONDS;
            }

            throw new Exception();
        }

        private int GetDigitMin()
        {
            return 0;
        }

        private int GetDigitMax(Digits digits)
        {
            // TODO: You'll need to change our max values depending on the current format we have
            switch (digits)
            {
                case Digits.DAYS:
                    return 99;
                case Digits.HOURS:
                    return 24;
                case Digits.MINUTES:
                    return 59;
                case Digits.SECONDS:
                    return 59;
                case Digits.MILLISECONDS:
                    return 10; // TODO: Should be a thousand but we are limited to double 
                default:
                    return 99;
            }
        }

        /// <summary>
        /// Shows this gameobject
        /// </summary>
        public void Show()
        {
            gameObject.SetActive(true);
        }

        /// <summary>
        /// Hides this gameobject
        /// </summary>
        public void Hide()
        {
            gameObject.SetActive(false);
        }

        /// <summary>
        /// Applies our theme changes to our components when necessary
        /// </summary>
        /// <param name="theme"></param>
        public override void ColorUpdate(Theme theme)
        {
            // Digits
            SetDigitColor(theme.GetCurrentColorScheme().m_foreground);

            // Separators
            foreach (DigitSeparator separator in generatedSeparators)
            {
                separator.SetSeparatorColor(theme.GetCurrentColorScheme().m_foreground);
            }
        }

        /// <summary>
        /// Sets the color of our digits to the provided color. <see cref="newColor"/>
        /// </summary>
        /// <param name="newColor"></param>
        public void SetDigitColor(Color newColor)
        {
            foreach (DoubleDigit digit in generatedDigits)
            {
                digit.SetTextColor(newColor);
            }
        }

        /// <summary>
        /// Allows our generated digits to be interacted with
        /// </summary>
        public void Unlock()
        {
            foreach (DoubleDigit digit in generatedDigits)
            {
                digit.Unlock();
            }
        }

        /// <summary>
        /// Prevents / disallows our generated digits to be interacted with
        /// </summary>
        public void Lock()
        {
            foreach (DoubleDigit digit in generatedDigits)
            {
                digit.Lock();
            }
        }

        /// <summary>
        /// Moves all our generated digits into their default viewport positions & anchors
        /// </summary>
        public void ResetTextPositions()
        {
            foreach (DoubleDigit digit in generatedDigits)
            {
                digit.ResetTextPosition();
            }
        }

        /// <summary>
        /// Returns our list of generated digits
        /// </summary>
        /// <returns></returns>
        public List<DoubleDigit> GetDigits()
        {
            return generatedDigits;
        }

        /// <summary>
        /// Returns our timer in string format
        /// </summary>
        /// <returns></returns>
        public string GetTimerString()
        {
            string result = String.Empty;

            for (int i = 0; i < generatedDigits.Count; i++)
            {
                DoubleDigit digit = generatedDigits[i];
                result += digit.GetDigitsLabel();

                // Skip last iteration
                if (i == generatedDigits.Count - 1)
                {
                    break;
                }

                result += ":";
            }

            return result;
        }

        /// <summary>
        /// Prevents tick animation from holding
        /// </summary>
        public void CorrectTickAnimVisuals()
        {
            foreach (DoubleDigit digit in generatedDigits)
            {
                if (digit.IsTickAnimationPlaying())
                {
                    digit.ResetTextPosition();
                }
            }
        }

        /// <summary>
        /// Increments the provided digit by one. (+1)
        /// </summary>
        /// <param name="digits"></param>
        public void IncrementOne(Digits digits)
        {
            SetDigit(digits, GetDigitValue(digits) + 1);
        }

        /// <summary>
        /// Decrements the provided digit by one. (-1)
        /// </summary>
        /// <param name="digits"></param>
        public void DecrementOne(Digits digits)
        {
            SetDigit(digits, GetDigitValue(digits) - 1);
        }

        /// <summary>
        /// Sets the provided digit to it's provided value
        /// </summary>
        /// <param name="digit"></param>
        /// <param name="newValue"></param>
        public void SetDigit(Digits digit, int newValue)
        {
            if (!m_isOnBreak)
            {
                m_workTime[(int)digit] = newValue;
            }
            else
            {
                m_breakTime[(int)digit] = newValue;
            }

            OnValidate();
        }

        /// <summary>
        /// Sets the value of the timer using the provided formatted string.
        /// </summary>
        /// <param name="formattedString">Expected format of ("00:25:00")</param>
        public void SetTimerValue(string formattedString)
        {
            // Only allow 'Set Timer Value' to work when we are in the setup state
            if (Timer.m_state != PomodoroTimer.States.SETUP)
            {
                return;
            }

            List<string> sections = new List<string>();
            string value = String.Empty;

            // ReSharper disable once InconsistentNaming
            // ReSharper disable once ForCanBeConvertedToForeach
            // Iterate through each character to determine how many sections there are...
            for (int i = 0; i < formattedString.Length; i++)
            {
                // If this character is a separator...
                if (formattedString[i].ToString() == ":") // TODO: Allow the use of other separators like '-'?
                {
                    // Save section
                    if (value != String.Empty)
                    {
                        sections.Add(value);
                        value = string.Empty;
                    }

                    continue;
                }

                // Add to section
                value += formattedString[i].ToString();
            }

            // Last digit in string won't have a separator so we add the section in once the loop is complete
            sections.Add(value);

            // Compare sections with timer format
            if (sections.Count != generatedDigits.Count)
            {
                Debug.LogWarning("The provided string does not match the pomodoro timer layout exactly. " +
                                 "Truncation may occur.");
            }

            // Set timer sections
            // Check if we have enough generated digits...(We will allow users to paste longer values, but only
            // carry over the values that can fit within the number of generated digits we have)
            if (sections.Count >= generatedDigits.Count)
            {
                for (int i = 0; i < generatedDigits.Count; i++)
                {
                    generatedDigits[i].SetValue(int.Parse(sections[i]));
                    generatedDigits[i].UpdateVisuals(GetDigitValue(generatedDigits[i].m_digit));
                }
            }
        }

        public int GetDigitValue(Digits digits)
        {
            if (!m_isOnBreak)
            {
                return m_workTime[(int)digits];
            }

            return m_breakTime[(int)digits];
        }

        /// <summary>
        /// Returns True if you can add one to this digit without hitting it's ceiling, otherwise returns False.
        /// </summary>
        /// <param name="digits"></param>
        /// <returns></returns>
        public bool CanIncrementOne(Digits digits)
        {
            if (GetDigitValue(digits) + 1 > GetDigitMax(digits))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Returns True if you can subtract one to this digit without hitting it's floor, otherwise returns False.
        /// </summary>
        /// <param name="digits"></param>
        /// <returns></returns>
        public bool CanDecrementOne(Digits digits)
        {
            if (GetDigitValue(digits) - 1 < GetDigitMin())
            {
                return false;
            }

            return true;
        }


        public void ClearTimerSelection()
        {
            Timer.ClearSelection();
        }

        public PomodoroTimer.States GetTimerState()
        {
            return Timer.m_state;
        }

        public void SetTimerSelection(DoubleDigit digitToSelect)
        {
            Timer.SetSelection(digitToSelect);
        }

        /// <summary>
        /// Switches your format to the provided format
        /// </summary>
        /// <param name="desiredFormat"></param>
        public void SwitchFormat(SupportedFormats desiredFormat)
        {
            previousFormatSelection = (int) m_format;
            m_format = desiredFormat;
        }

        /// <summary>
        /// Returns the previously selected format index
        /// </summary>
        /// <returns></returns>
        public int GetPreviousFormatSelection()
        {
            return previousFormatSelection;
        }
        
        /// <summary>
        /// Return the current format index
        /// </summary>
        /// <returns></returns>
        public int GetFormat()
        {
            return (int) m_format;
        }
    }
 }