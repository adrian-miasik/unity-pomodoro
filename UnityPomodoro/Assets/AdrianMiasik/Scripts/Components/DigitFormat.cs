using System.Collections.Generic;
using UnityEngine;

namespace AdrianMiasik.Components
{
    public class DigitFormat : MonoBehaviour
    {
        // TODO: Formats to support
        private enum SupportedFormats
        {
            DD_HH_MM_SS_MS, // Full
            HH_MM_SS_MS,    // Detail
            HH_MM_SS,       // Default / Standard
            MM_SS,          // Simple
            SS              // Bare
        }

        [SerializeField] private SupportedFormats format;
        private List<DoubleDigit> digits;

        private void Initialize(PomodoroTimer _timer)
        {
            char[] _separatorChar = { '_' };
            GenerateDoubleDigits(GetDoubleDigitCountFromFormat(format, _separatorChar[0]));
        }

        /// <summary>
        /// Returns the number of double digits within a supported format. <see cref="SupportedFormats"/>
        /// </summary>
        /// <param name="_format">The format in question</param>
        /// <param name="_separator">What character is separating digits from each other?</param>
        /// <returns></returns>
        private int GetDoubleDigitCountFromFormat(SupportedFormats _format, char _separator)
        {
            string _formatString = _format.ToString();
            int _numberOfDoubleDigits = 1;

            // Iterate through each character in enum
            foreach (char _c in _formatString)
            {
                if (_c == _separator)
                {
                    _numberOfDoubleDigits++;
                }
            }

            return _numberOfDoubleDigits;
        }

        private void GenerateDoubleDigits(int _howManyDoubleDigits)
        {
            Debug.Log("Generating " + _howManyDoubleDigits + " digit pairs.");
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
 }