using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dtk.Eventing.EventAggregation {

	public class DefaultEventAggregator : IEventAggregator {

		private IEnumerable<Subscription> _subscriptions;

		public DefaultEventAggregator() => _subscriptions = new List<Subscription>();

		public int SubscriptionCount => _subscriptions.Count();

		public async Task PublishAsync<T>(T @event) where T : IEvent {
			var handlersToInvoke = _subscriptions
				.Where(s => s.EventName == typeof(T).Name)
				.Select(s => s.Handler);
			await Task.WhenAll(handlersToInvoke.Select(handler => handler(@event)));
		}

		public Guid Register<TEvent>(Func<TEvent, Task> callback) where TEvent : IEvent {
			Guid subscriptionId = Guid.NewGuid();
			_subscriptions = _subscriptions.Concat(new[] {
				new Subscription() {
					Id = subscriptionId,
					EventName = typeof(TEvent).Name,
					Handler = (e) => callback((TEvent)e)
				}
			});
			return subscriptionId;
		}

		public void Unsubscribe(Guid subscriptionId) =>
			_subscriptions = _subscriptions.Where(s => s.Id != subscriptionId);

		private class Subscription {
			internal string EventName { get; set; } = String.Empty;
			internal Guid Id { get; set; } = Guid.Empty;
			internal Func<IEvent, Task> Handler { get; set; } = (e) =>  Task.CompletedTask;
		}

	}

}
