using System;
using System.Collections.Generic;
using AdrianMiasik.Interfaces;
using AdrianMiasik.ScriptableObjects;
using UnityEngine;
using UnityEngine.UI;

namespace AdrianMiasik.Components
{
    public class DigitFormat : MonoBehaviour, IColorHook
    {
        public enum Digits
        {
            DAYS,
            HOURS,
            MINUTES,
            SECONDS,
            MILLISECONDS
        }
        
        private enum SupportedFormats
        {
            DD_HH_MM_SS_MS, // Full
            HH_MM_SS_MS,    // Detail
            HH_MM_SS,       // Default / Standard
            MM_SS,          // Simple
            SS              // Bare
        }
        
        // Current format
        [SerializeField] private SupportedFormats format;

        [Header("Source Prefabs")]
        [SerializeField] private DoubleDigit digitSource;
        [SerializeField] private DigitSeparator separatorSource;

        [Header("Work Data")]
        [SerializeField] private int[] workTime = new int[5]{0,0,25,0,0}; // Represents data for DD_HH_MM_SS_MS

        [Header("Break Data")]
        public bool isOnBreak;
        [SerializeField] private int[] breakTime = new int[5]{0,0,5,0,0}; // Represents data for DD_HH_MM_SS_MS

        // Cache
        private PomodoroTimer timer;
        private Theme theme;
        private List<DoubleDigit> generatedDigits;
        private List<DigitSeparator> generatedSeparators;
        private TimeSpan cachedTimeSpan;
        
        private void OnValidate()
        {
            // Prevent values from going over their limit
            if (!isOnBreak)
            {
                for (int _i = 0; _i < workTime.Length; _i++)
                {
                    workTime[_i] = Mathf.Clamp(workTime[_i], GetDigitMin(), GetDigitMax((Digits)_i));
                }
            }
            else
            {
                for (int _i = 0; _i < breakTime.Length; _i++)
                {
                    breakTime[_i] = Mathf.Clamp(breakTime[_i], GetDigitMin(), GetDigitMax((Digits)_i));
                }
            }
        }
        
        public void Initialize(PomodoroTimer _timer, Theme _theme)
        {
            timer = _timer;
            theme = _theme;
            theme.RegisterColorHook(this);
            
            GenerateFormat();
        }

        [ContextMenu("Generate Format")]
        public void GenerateFormat()
        {
            generatedDigits?.Clear();
            generatedSeparators?.Clear();

            // Clear pre-generated digits / transforms
            foreach (Transform _t in transform.GetComponentsInChildren<Transform>())
            {
                if (_t == transform)
                {
                    continue;
                }
                
                Destroy(_t.gameObject);
            }
            
            // Generate our digits and separators (format)
            char[] _separatorChar = { '_' };
            GenerateFormat(GetDoubleDigitSet(format, _separatorChar[0]));

            // Calculate time
            cachedTimeSpan = GetTime();
            SetTime(cachedTimeSpan);
            
            // Apply
            RefreshDigitVisuals();
        }

        public void RefreshDigitVisuals()
        {
            foreach (DoubleDigit _digit in generatedDigits)
            {
                _digit.UpdateVisuals(GetDigitValue(_digit.digit));
                _digit.HideArrows();
            }
        }

        public void FlipIsOnBreakBool()
        {
            isOnBreak = !isOnBreak;
        }
        
        public TimeSpan GetTime()
        {
            TimeSpan _ts;
            if (!isOnBreak)
            {
                _ts = TimeSpan.FromDays(workTime[0]) +
                      TimeSpan.FromHours(workTime[1]) +
                      TimeSpan.FromMinutes(workTime[2]) +
                      TimeSpan.FromSeconds(workTime[3]) +
                      TimeSpan.FromMilliseconds(workTime[4]);
            }
            else
            {
                _ts = TimeSpan.FromDays(breakTime[0]) +
                      TimeSpan.FromHours(breakTime[1]) +
                      TimeSpan.FromMinutes(breakTime[2]) +
                      TimeSpan.FromSeconds(breakTime[3]) +
                      TimeSpan.FromMilliseconds(breakTime[4]);
            }

            return _ts;
        }

        /// <summary>
        /// Returns a list of key value pairs using the provided enum format.
        /// Each key string determines it's type (such as hour/minute/second/etc...) and,
        /// each value int determines it's starting value.
        /// </summary>
        /// <param name="_format"></param>
        /// <param name="_separator"></param>
        /// <returns></returns>
        private List<KeyValuePair<string, int>> GetDoubleDigitSet(SupportedFormats _format, char _separator)
        {
            string _formatString = _format.ToString();
            string _currentAbbreviation = String.Empty;

            List<KeyValuePair<string, int>> _result = new List<KeyValuePair<string, int>>();

            // Iterate through each character in enum
            for (int _i = 0; _i < _formatString.Length; _i++)
            {
                char _currentCharacter = _formatString[_i];

                // If we hit our separator...
                if (_currentCharacter == _separator)
                {
                    // Cache our abbreviation and value
                    _result.Add(new KeyValuePair<string, int>(_currentAbbreviation, 0)); // Starting value of zero
                    _currentAbbreviation = String.Empty;
                }
                else
                {
                    _currentAbbreviation += _currentCharacter;
                    
                    // If we reached the end of our list...
                    if (_i == _formatString.Length - 1)
                    {
                        _result.Add(new KeyValuePair<string, int>(_currentAbbreviation, 0)); // Starting value of zero
                    }
                }
            }

            // Return abbreviation and value set
            return _result;
        }
        
        /// <summary>
        /// Generates the appropriate amount of digits and separators based on the provided data set
        /// </summary>
        /// <param name="_doubleDigitSetToGenerate">String represents abbreviation, and int represents value</param>
        private void GenerateFormat(List<KeyValuePair<string, int>> _doubleDigitSetToGenerate)
        {
            generatedDigits = new List<DoubleDigit>();
            generatedSeparators = new List<DigitSeparator>();

            for (int _i = 0; _i < _doubleDigitSetToGenerate.Count; _i++)
            {
                KeyValuePair<string, int> _pair = _doubleDigitSetToGenerate[_i];
                
                // Generate double digit
                DoubleDigit _dd = Instantiate(digitSource, transform);
                _dd.Initialize(this, GetDigitType(_pair.Key), _pair.Value, theme);
                generatedDigits.Add(_dd);

                // Skip last iteration to avoid spacer generation
                if (_i == _doubleDigitSetToGenerate.Count - 1)
                {
                    break;
                }
                
                // Generate spacer (between each character)
                DigitSeparator _separator = Instantiate(separatorSource, transform);
                generatedSeparators.Add(_separator);
            }

            // Hook up selectable navigations
            for (int _i = 0; _i < generatedDigits.Count; _i++)
            {
                DoubleDigit _digit = generatedDigits[_i];
                
                // Create navigation
                Navigation _digitNav = new Navigation
                {
                    mode = Navigation.Mode.Explicit,
                    selectOnLeft = generatedDigits[Wrap(_i - 1, generatedDigits.Count)].GetSelectable(),
                    selectOnRight = generatedDigits[Wrap(_i + 1, generatedDigits.Count)].GetSelectable()
                };

                // Apply navigation
                _digit.GetSelectable().navigation = _digitNav;
            }
            
            // Fix background navigation
            Navigation _backgroundNav = new Navigation
            {
                mode = Navigation.Mode.Explicit,
                selectOnRight = generatedDigits[0].GetSelectable(),
                selectOnLeft = generatedDigits[generatedDigits.Count - 1].GetSelectable()
            };
            timer.SetBackgroundNavigation(_backgroundNav);
        }

        private int Wrap(int _index, int _length)
        {
            return (_index % _length + _length) % _length;
        }
        
        public void ShowTime(TimeSpan _ts)
        {
            foreach (DoubleDigit _doubleDigit in generatedDigits)
            {
                switch (_doubleDigit.digit)
                {
                    case Digits.DAYS:
                        _doubleDigit.SetDigitsLabel(_ts.Days);
                        break;
                    case Digits.HOURS:
                        _doubleDigit.SetDigitsLabel(_ts.Hours);
                        break;
                    case Digits.MINUTES:
                        _doubleDigit.SetDigitsLabel(_ts.Minutes);
                        break;
                    case Digits.SECONDS:
                        _doubleDigit.SetDigitsLabel(_ts.Seconds);
                        break;
                    case Digits.MILLISECONDS:
                        _doubleDigit.SetDigitsLabel(_ts.Milliseconds);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public void SetTime(TimeSpan _ts)
        {
            // Clear out time arrays
            if (!isOnBreak)
            {
                Array.Clear(workTime, 0, workTime.Length);
            }
            else
            {
                Array.Clear(breakTime, 0, breakTime.Length);
            }

            // Apply time arrays to update only for generated digits (essentially throwing out data that's not used)
            foreach (DoubleDigit _doubleDigit in generatedDigits)
            {
                Digits _type = _doubleDigit.digit;
                
                switch (_type)
                {
                    case Digits.DAYS:
                        _doubleDigit.SetValue(_ts.Days);
                        break;
                    case Digits.HOURS:
                        _doubleDigit.SetValue(_ts.Hours);
                        break;
                    case Digits.MINUTES:
                        _doubleDigit.SetValue(_ts.Minutes);
                        break;
                    case Digits.SECONDS:
                        _doubleDigit.SetValue(_ts.Seconds);
                        break;
                    case Digits.MILLISECONDS:
                        _doubleDigit.SetValue(_ts.Milliseconds);
                        break;
                } 
            }
        }

        /// <summary>
        /// Returns the digit type using the provided abbreviation string
        /// </summary>
        /// <param name="_abbreviation"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private static Digits GetDigitType(string _abbreviation)
        {
            switch (_abbreviation)
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

        private int GetDigitMax(Digits _digits)
        {
            // TODO: You'll need to change our max values depending on the current format we have
            switch (_digits)
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
        /// <param name="_theme"></param>
        public void ColorUpdate(Theme _theme)
        {
            // Digits
            SetDigitColor(_theme.GetCurrentColorScheme().foreground);
            
            // Separators
            foreach (DigitSeparator _separator in generatedSeparators)
            {
                _separator.SetSeparatorColor(_theme.GetCurrentColorScheme().foreground);
            }
        }
        
        /// <summary>
        /// Sets the color of our digits to the provided color. <see cref="_newColor"/>
        /// </summary>
        /// <param name="_newColor"></param>
        public void SetDigitColor(Color _newColor)
        {
            foreach (DoubleDigit _digit in generatedDigits)
            {
                _digit.SetTextColor(_newColor);
            }
        }

        /// <summary>
        /// Allows our generated digits to be interacted with
        /// </summary>
        public void Unlock()
        {
            foreach (DoubleDigit _digit in generatedDigits)
            {
                _digit.Unlock();
            }
        }
        
        /// <summary>
        /// Prevents / disallows our generated digits to be interacted with
        /// </summary>
        public void Lock()
        {
            foreach (DoubleDigit _digit in generatedDigits)
            {
                _digit.Lock();
            }
        }

        /// <summary>
        /// Moves all our generated digits into their default viewport positions & anchors
        /// </summary>
        public void ResetTextPositions()
        {
            foreach (DoubleDigit _digit in generatedDigits)
            {
                _digit.ResetTextPosition();
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
            string _result = String.Empty;

            for (int _i = 0; _i < generatedDigits.Count; _i++)
            {
                DoubleDigit _digit = generatedDigits[_i];
                _result += _digit.GetDigitsLabel();

                // Skip last iteration
                if (_i == generatedDigits.Count - 1)
                {
                    break;
                }
                _result += ":";
            }

            return _result;
        }

        /// <summary>
        /// Prevents tick animation from holding
        /// </summary>
        public void CorrectTickAnimVisuals()
        {
            foreach (DoubleDigit _digit in generatedDigits)
            {
                if (_digit.IsTickAnimationPlaying())
                {
                    _digit.ResetTextPosition();
                }
            }
        }
        
        /// <summary>
        /// Increments the provided digit by one. (+1)
        /// </summary>
        /// <param name="_digits"></param>
        public void IncrementOne(Digits _digits)
        {
            SetDigit(_digits, GetDigitValue(_digits) + 1);
        }

        /// <summary>
        /// Decrements the provided digit by one. (-1)
        /// </summary>
        /// <param name="_digits"></param>
        public void DecrementOne(Digits _digits)
        {
            SetDigit(_digits, GetDigitValue(_digits) - 1);
        }
        
        /// <summary>
        /// Sets the provided digit to it's provided value
        /// </summary>
        /// <param name="_digit"></param>
        /// <param name="_newValue"></param>
        public void SetDigit(Digits _digit, int _newValue)
        {
            if (!isOnBreak)
            {
                workTime[(int)_digit] = _newValue;
            }
            else
            {
                breakTime[(int)_digit] = _newValue;
            }

            OnValidate();
        }

        // TODO: Fix this functionality
        /// <summary>
        /// Sets the value of the timer using the provided formatted string.
        /// </summary>
        /// <param name="_formattedString">Expected format of ("00:25:00")</param>
        public void SetTimerValue(string _formattedString)
        {
            // Only allow 'Set Timer Value' to work when we are in the setup state
            if (GetTimer().state != PomodoroTimer.States.SETUP)
            {
                return;
            }
            
            List<string> _sections = new List<string>();
            string _value = String.Empty;

            // ReSharper disable once InconsistentNaming
            // ReSharper disable once ForCanBeConvertedToForeach
            // Iterate through each character to determine how many sections there are...
            for (int i = 0; i < _formattedString.Length; i++)
            {
                // If this character is a separator...
                if (_formattedString[i].ToString() == ":")
                {
                    // Save section
                    if (_value != String.Empty)
                    {
                        _sections.Add(_value);
                        _value = string.Empty;
                    }

                    continue;
                }

                // Add to section
                _value += _formattedString[i].ToString();
            }

            // Last digit in string won't have a separator so we add the section in once the loop is complete
            _sections.Add(_value);

            // Compare sections with timer format
            if (_sections.Count != 3) // TODO: Support more digit formats
            {
                Debug.LogWarning("The provided string does not match the pomodoro timer layout");
                return;
            }

            // Set timer sections
            // ReSharper disable once InconsistentNaming
            for (int i = 0; i < _sections.Count; i++)
            {
                switch (i)
                {
                    case 0:
                        // SetHours(_sections[i]);
                        break;
                    case 1:
                        // SetMinutes(_sections[i]);
                        break;
                    case 2:
                        // SetSeconds(_sections[i]);
                        break;
                }
            }
        }
        
        public int GetDigitValue(Digits _digits)
        {
            if (!isOnBreak)
            {
                return workTime[(int)_digits];
            }

            return breakTime[(int)_digits];
        }
        
        /// <summary>
        /// Returns True if you can add one to this digit without hitting it's ceiling, otherwise returns False.
        /// </summary>
        /// <param name="_digits"></param>
        /// <returns></returns>
        public bool CanIncrementOne(Digits _digits)
        {
            if (GetDigitValue(_digits) + 1 > GetDigitMax(_digits))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Returns True if you can subtract one to this digit without hitting it's floor, otherwise returns False.
        /// </summary>
        /// <param name="_digits"></param>
        /// <returns></returns>
        public bool CanDecrementOne(Digits _digits)
        {
            if (GetDigitValue(_digits) - 1 < GetDigitMin())
            {
                return false;
            }

            return true;
        }

        public PomodoroTimer GetTimer()
        {
            return timer;
        }
    }
 }