using System;
using System.Collections.Generic;
using AdrianMiasik.Interfaces;
using AdrianMiasik.ScriptableObjects;
using UnityEngine;

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

        private PomodoroTimer timer;
        
        [SerializeField] private SupportedFormats format;
        private KeyValuePair<string, int> doubleDigitSet;

        [Tooltip("This is the digit prefab we will be cloning from.")]
        [SerializeField] private DoubleDigit digitSource;
        
        [Tooltip("This is the separator prefab we will be cloning from.")]
        [SerializeField] private DigitSeparator separatorSource;
        
        private List<DoubleDigit> generatedDigits;
        private List<DigitSeparator> generatedSeparators;

        public void Initialize(PomodoroTimer _timer, Theme _theme)
        {
            timer = _timer;
            _theme.RegisterColorHook(this);
            
            // Clear pre-generated digits
            foreach (Transform _t in transform.GetComponentsInChildren<Transform>())
            {
                if (_t == transform)
                {
                    continue;
                }
                
                Destroy(_t.gameObject);
            }
            
            // Generate our digits
            char[] _separatorChar = { '_' };
            GenerateFormat(GetDoubleDigitSet(format, _separatorChar[0]));
        }
        
        /// <summary>
        /// Returns a list of key value pairs using the provided enum format.
        /// Each key string determines it's type (such as hour/minute/secound/etc...) and,
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

        private void GenerateFormat(List<KeyValuePair<string, int>> _doubleDigitSetToGenerate)
        {
            generatedDigits = new List<DoubleDigit>();
            generatedSeparators = new List<DigitSeparator>();

            for (int _i = 0; _i < _doubleDigitSetToGenerate.Count; _i++)
            {
                KeyValuePair<string, int> _pair = _doubleDigitSetToGenerate[_i];
                
                // Generate double digit
                DoubleDigit _dd = Instantiate(digitSource, transform);
                _dd.Initialize(GetDigitType(_pair.Key), timer, _pair.Value);
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
        }

        public void SetFormatTime(TimeSpan _ts)
        {
            foreach (DoubleDigit _digit in generatedDigits)
            {
                Digits _type = _digit.GetDigit();
                switch (_type)
                {
                    case Digits.DAYS:
                        _digit.SetValue(_ts.Days);
                        break;
                    case Digits.HOURS:
                        _digit.SetValue(_ts.Hours);
                        break;
                    case Digits.MINUTES:
                        _digit.SetValue(_ts.Minutes);
                        break;
                    case Digits.SECONDS:
                        _digit.SetValue(_ts.Seconds);
                        break;
                    case Digits.MILLISECONDS:
                        _digit.SetValue(_ts.Milliseconds);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private Digits GetDigitType(string _abbreviation)
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
                default:
                    Debug.LogAssertion("Unsupported abbreviation string!");
                    throw new Exception();
            }
        }
        
        public int GetDigitMin()
        {
            return 0;
        }
        
        public int GetDigitMax(DigitFormat.Digits _digits)
        {
            switch (_digits)
            {
                case DigitFormat.Digits.HOURS:
                    return 99;
                case DigitFormat.Digits.MINUTES:
                    return 59;
                case DigitFormat.Digits.SECONDS:
                    return 59;
                default:
                    throw new ArgumentOutOfRangeException(nameof(_digits), _digits, null);
            }
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

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

        public void Unlock()
        {
            foreach (DoubleDigit _digit in generatedDigits)
            {
                _digit.Unlock();
            }
        }
        
        public void Lock()
        {
            foreach (DoubleDigit _digit in generatedDigits)
            {
                _digit.Unlock();
            }
        }

        public void ResetTextPositions()
        {
            foreach (DoubleDigit _digit in generatedDigits)
            {
                _digit.ResetTextPosition();
            }
        }

        public List<DoubleDigit> GetDigits()
        {
            return generatedDigits;
        }

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
    }
 }