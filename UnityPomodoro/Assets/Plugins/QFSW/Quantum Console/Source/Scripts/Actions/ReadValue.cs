using System;
using System.Collections.Generic;

namespace QFSW.QC.Actions
{
    /// <summary>
    /// Gets the next line of text entered into the console as a user response 
    /// and parses it to a value of the specified type.
    /// </summary>
    public class ReadValue<T> : Composite
    {
        private static readonly QuantumParser Parser = new QuantumParser();

        /// <param name="getValue">A delegate which returns the parsed value entered by the user.</param>
        /// <param name="config">The config to provide the response flow with.</param>
        public ReadValue(Action<T> getValue, ResponseConfig config)
            : base(Generate(getValue, config))
        { }

        /// <param name="getValue">A delegate which returns the parsed value entered by the user.</param>
        public ReadValue(Action<T> getValue)
            : this(getValue, ResponseConfig.Default)
        { }

        private static IEnumerator<ICommandAction> Generate(Action<T> getValue, ResponseConfig config)
        {
            string line = default;
            yield return new ReadLine(t => line = t, config);

            T value = Parser.Parse<T>(line);
            getValue(value);
        }
    }
}
