using System;
using System.Threading.Tasks;

namespace Dtk.Extensions {

    public static class TaskExtensions {

		public static async void FireAndForgetSafe(this Task task, Action<Exception> onError, Action onFinally) {
			try {
				await task;
			}
			catch (Exception err) {
				onError(err);
			}
			finally {
				onFinally();
			}
		}

    }

}
