using System;

namespace QFSW.QC.Actions
{
    /// <summary>
    /// Gets the next line of text entered into the console as a user 
    /// response instead of invoking it as a command.
    /// </summary>
    public class ReadLine : ICommandAction
    {
        private readonly Action<string> _getInput;
        private readonly ResponseConfig _config;
        private QuantumConsole _console;
        private string _response;

        public bool IsFinished => _response != null;

        public bool StartsIdle => true;

        /// <param name="getInput">A delegate which returns the input entered by the user.</param>
        /// <param name="config">The config to provide the response flow with.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public ReadLine(Action<string> getInput, ResponseConfig config)
        {
            // validate
            if (getInput == null)
            {
                throw new ArgumentNullException(nameof(getInput));
            }

            // set fields
            _getInput = getInput;
            _config = config;
            _console = null;
            _response = null;
        }

        /// <param name="getInput">A delegate which returns the input entered by the user.</param>
        /// <param name="config">The config to provide the response flow with.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public ReadLine(Action<string> getInput) : this(getInput, ResponseConfig.Default)
        {

        }

        public void Finalize(ActionContext context)
        {
            _getInput(_response); // push value to the caller
        }

        public void Start(ActionContext context)
        {
            _response = null; // reset flag
            _console = context.Console;
            _console.BeginResponse(OnResponseSubmittedHandler, _config);
        }

        private void OnResponseSubmittedHandler(string response)
        {
            _response = response; // changes IsFinished flag
        }
    }
}
