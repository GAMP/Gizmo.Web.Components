using System.Threading.Tasks;
using System.Windows.Input;

namespace Gizmo.Web.Components
{
    public abstract class DOMCommandComponentBase<TArgument> : CustomDOMComponentBase
    {
        #region FIELDS

        private IAsyncCommand<TArgument> _command;
        private TArgument _commandParameter;

        #endregion

        #region PROPERTIES

        /// <summary>
        /// Gets or sets command.
        /// </summary>
        public IAsyncCommand<TArgument> Command
        {
            get { return _command; }
            set { _command = value; }
        }

        /// <summary>
        /// Gets or sets command parameter.
        /// </summary>
        public TArgument CommandParameter
        {
            get { return _commandParameter; }
            set { _commandParameter = value; }
        }

        #endregion

        #region FUNCTIONS

        #region PROTECTED VIRTUAL

        protected Task CommandExecuteCoreAsync()
        {
            return CommandExecuteCoreAsync(CommandParameter);
        }

        protected virtual async Task CommandExecuteCoreAsync(TArgument param)
        {
            var targetCommand = Command;
            if (targetCommand == null)
                return;

            if (!targetCommand.CanExecute(param))
                return;

            await targetCommand.ExecuteAsync(param);
        }  

        #endregion
        
        #endregion
    }
}
