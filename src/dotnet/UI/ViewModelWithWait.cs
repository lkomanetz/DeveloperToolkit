using System;
using System.Threading.Tasks;

namespace Dtk.UI {

	public abstract class ViewModelWithWait : BaseViewModel {

		private bool _isWaiting;
		public bool IsWaiting {
			get => _isWaiting;
			set => this.SetPropertyValue(
				vm => vm.IsWaiting,
				value,
				ref _isWaiting,
				FirePropertyChanged
			);
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
