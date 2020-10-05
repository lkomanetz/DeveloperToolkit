using System;
using System.Threading.Tasks;

namespace Dtk.UI {

	public abstract class ViewModelWithWait : BaseViewModel {

		private bool _isWaiting;
		public bool IsWaiting {
			get => _isWaiting;
			set => this.SetProperty(value, ref _isWaiting);
		}

		protected async Task DoWhileWaitingAsync(Func<Task> func) {
			IsWaiting = true;
			try {
				await func();
			}
			finally {
				IsWaiting = false;
			}
		}

	}

}
