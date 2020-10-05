using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace Dtk.UI.Commands {

    public class DelegateCommand : CommandBase {

        private Action _action;
        private Action<Exception> _errorHandler;

        public DelegateCommand(Action action, Action<Exception> errorHandler) => (_action, _errorHandler) = (action, errorHandler);

        protected override void Execute(object parameter) => Attempt(_action, _errorHandler);

    }

}
