using System;
using System.Windows.Input;

namespace Gizmo.Web.Components.Infrastructure
{
    public class SimpleCommand<T1, T2> : ICommand
    {
        public SimpleCommand(Func<T1, bool> canExecuteMethod, Action<T2> executeMethod)
        {
            _executeMethod = executeMethod;
            _canExecuteMethod = canExecuteMethod;
        }

        public SimpleCommand(Action<T2> executeMethod)
        {
            _executeMethod = executeMethod;
            _canExecuteMethod = (x) => { return true; };
        }

        public event EventHandler CanExecuteChanged;

        private Func<T1, bool> _canExecuteMethod;
        private Action<T2> _executeMethod;

        public bool CanExecute(object parameter)
        {
            try
            {
                if (_canExecuteMethod == null) return true;
                return _canExecuteMethod((T1)parameter);
            }
            catch
            {
                throw;
            }
        }

        public void Execute(object parameter)
        {
            try
            {
                _executeMethod?.Invoke((T2)parameter);
            }
            catch
            {
                throw;
            }
        }
    }
}
