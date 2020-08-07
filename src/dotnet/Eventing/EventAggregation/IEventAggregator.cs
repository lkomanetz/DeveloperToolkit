using System;
using System.Threading.Tasks;

namespace Dtk.Eventing.EventAggregation {

	public interface IEventAggregator {

		int SubscriptionCount { get; }

		Guid Register<T>(Func<T, Task> callback) where T: IEvent;
		Task PublishAsync<T>(T @event) where T : IEvent;
		void Unsubscribe(Guid subscriptionId);

	}

}
