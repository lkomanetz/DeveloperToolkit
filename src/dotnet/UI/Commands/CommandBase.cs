using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace Dtk.UI.Commands {

    public abstract class CommandBase : ICommand {

        public event EventHandler CanExecuteChanged = delegate { };

        bool ICommand.CanExecute(object parameter) => CanExecute(parameter);
        void ICommand.Execute(object parameter) => Execute(parameter);
        protected abstract void Execute(object parameter);
        protected virtual bool CanExecute(object parameter) => true;

        protected void Attempt(Action action, Action<Exception> errorHandler) {
            try { 
                action();
            }
            catch (Exception err) {
                errorHandler.Invoke(err);
            }
        }

    }

}
