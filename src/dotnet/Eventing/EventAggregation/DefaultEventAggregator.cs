using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dtk.Eventing.EventAggregation {

	public class DefaultEventAggregator : IEventAggregator {

		private readonly ConcurrentDictionary<Guid, Subscription> _subscriptions;

        public DefaultEventAggregator() => _subscriptions = new ConcurrentDictionary<Guid, Subscription>();

        public int SubscriptionCount => _subscriptions.Count();

		public async Task PublishAsync<T>(T @event) where T : IEvent {
			var handlersToInvoke = FindHandlersFor(typeof(T));
			await Task.WhenAll(handlersToInvoke.Select(handler => handler(@event)));
		}

		public Guid Register<TEvent>(Func<TEvent, Task> callback) where TEvent : IEvent {
			Guid subscriptionId = Guid.NewGuid();
			var subscription = new Subscription() { EventName = typeof(TEvent).Name, Handler = (e) => callback((TEvent)e) };
			bool added = _subscriptions.TryAdd(subscriptionId, subscription);
			if (!added) throw new Exception($"Failed to register handler to '{typeof(TEvent).Name}' event.");
			return subscriptionId;
		}

		public void Unsubscribe(Guid subscriptionId) => _subscriptions.TryRemove(subscriptionId, out Subscription _);

		private IEnumerable<Func<IEvent, Task>> FindHandlersFor(Type t) => _subscriptions
			.Where(kvp => kvp.Value.EventName == t.Name)
			.Select(kvp => kvp.Value.Handler);

        private class Subscription {
            internal string EventName { get; set; } = String.Empty;
            internal Func<IEvent, Task> Handler { get; set; } = (e) => Task.CompletedTask;
        }

    }

}
