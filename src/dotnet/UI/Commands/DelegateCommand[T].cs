using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace Dtk.UI.Commands {

    public class DelegateCommand<T> : CommandBase {

        private readonly Action<T> _action;
        private readonly Action<Exception> _errorHandler;

        public DelegateCommand(Action<T> action, Action<Exception> errorHandler) => (_action, _errorHandler) = (action, errorHandler);

        protected override void Execute(object parameter) => Attempt(() => _action((T)parameter), _errorHandler);

    }

}
