using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Gizmo.Web.Components.Infrastructure
{
    //TODO: A
    public class SimpleAsyncCommand<T1, T2> : IAsyncCommand<T2>
    {
        public SimpleAsyncCommand(Func<object, Task> executeMethod)
        {
            _executeMethod = executeMethod;
        }

        private Func<object, Task> _executeMethod;

        public bool IsExecuting => throw new NotImplementedException();

        public bool AllowsMultipleExecutions => throw new NotImplementedException();

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
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

        public Task ExecuteAsync(T2 parameter)
        {
            return _executeMethod(parameter);
        }

        public void RaiseCanExecuteChanged()
        {
            throw new NotImplementedException();
        }
    }
}
