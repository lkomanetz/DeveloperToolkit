using Dtk.Extensions;
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Dtk.UI {

	public interface IAsyncCommand : ICommand {
		Task ExecuteAsync(object parameter);
		bool CanExecute();
	}
	
	public abstract class AsyncCommandBase : IAsyncCommand {

		private readonly Action<Exception> _errorHandler;

		public AsyncCommandBase(Action<Exception> errorHandler) => _errorHandler = errorHandler;

		public event EventHandler CanExecuteChanged = delegate { };

		private bool _isRunning;
		public bool IsRunning {
			get => _isRunning;
			private set {
				if (_isRunning == value) return;
				_isRunning = value;
				CanExecuteChanged(this, EventArgs.Empty);
			}
		}

		public bool CanExecute() => !IsRunning;
		bool ICommand.CanExecute(object parameter) => CanExecute();

		void ICommand.Execute(object parameter) {
			if (!CanExecute()) return;
			IsRunning = true;
			ExecuteAsync(parameter).FireAndForgetSafe(_errorHandler, () => IsRunning = false);
		}

		public abstract Task ExecuteAsync(object parameter);
		
	}

	public class AsyncDelegateCommand<T> : AsyncCommandBase {
		
		private readonly Func<T, Task> _func;

		public AsyncDelegateCommand(Func<T, Task> func, Action<Exception> errorHandler) : base(errorHandler) =>
			_func = func;

		public override Task ExecuteAsync(object parameter) => _func((T)parameter);

	}

	public class AsyncDelegateCommand : AsyncCommandBase {

		private readonly Func<Task> _func;

		public AsyncDelegateCommand(Func<Task> func, Action<Exception> errorHandler) : base(errorHandler) =>
			_func = func;

		public override Task ExecuteAsync(object parameter) => _func();

	}

}
