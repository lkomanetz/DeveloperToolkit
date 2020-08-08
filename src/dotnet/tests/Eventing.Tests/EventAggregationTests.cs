using Dtk.Eventing.EventAggregation;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Eventing.Tests {

    public class EventAggregationTests {

		private readonly IEventAggregator _aggregator;

		public EventAggregationTests() => _aggregator = new DefaultEventAggregator();

        [Theory]
		[InlineData(3)]
		[InlineData(1)]
		[InlineData(0)]
		[InlineData(10)]
        public void RegisterAddsToSubscriptions(int handlerCount) {
			for (int i = 0; i < handlerCount; ++i) {
				_aggregator.Register<TestEvent>((e) => Task.CompletedTask);
			}
			Assert.Equal(handlerCount, _aggregator.SubscriptionCount);
        }

		[Theory]
		[InlineData(5)]
		[InlineData(1)]
		[InlineData(0)]
		[InlineData(10)]
		public void UnsubscribeRemovesFromSubscriptions(int handlerCount) {
			var subscriptionIds = new Guid[handlerCount];
			for (int i = 0; i < handlerCount; ++i) {
				subscriptionIds[i] = _aggregator.Register<TestEvent>(e => Task.CompletedTask);
			}
			foreach (var id in subscriptionIds) _aggregator.Unsubscribe(id);
			Assert.Equal(0, _aggregator.SubscriptionCount);
		}

		[Fact]
		public async Task HandlerIsExecutedOnPublish() {
			var mockHandler = new Mock<Func<IEvent, Task>>();
			_aggregator.Register<TestEvent>(mockHandler.Object);
			await _aggregator.PublishAsync(new TestEvent() { SomeNumber = 1 });
			mockHandler.Verify(h => h.Invoke(It.IsAny<IEvent>()), Times.Once());
		}

		[Theory]
		[InlineData(2)]
		[InlineData(10)]
		[InlineData(50)]
		public async Task MultipleHandlersExecuted(int handlerCount) {
			int callCount = 0;
			var mocks = new Mock<Func<IEvent, Task>>[handlerCount];

			for (int i = 0; i < handlerCount; ++i) {
				mocks[i] = new Mock<Func<IEvent, Task>>();
				mocks[i].Setup(m => m.Invoke(It.IsAny<IEvent>())).Callback(() => ++callCount);
				_aggregator.Register<TestEvent>(mocks[i].Object);
			}

			await _aggregator.PublishAsync(new TestEvent() { SomeNumber = 1 });
			AssertEveryMockExecuted(mocks);
			Assert.Equal(handlerCount, callCount);
			
			static void AssertEveryMockExecuted(IEnumerable<Mock<Func<IEvent, Task>>> mockHandlers) {
				foreach (var m in mockHandlers) m.Verify(m => m.Invoke(It.IsAny<IEvent>()), Times.Once());
			}
		}

		[Fact]
		public async Task ExecutesOnlyRegisteredEvents() {
			var mockHandler = new Mock<Func<IEvent, Task>>();
			_aggregator.Register<OtherEvent>(mockHandler.Object);

			await Task.WhenAll(
				_aggregator.PublishAsync(new TestEvent()),
				_aggregator.PublishAsync(new OtherEvent())
			);

			mockHandler.Verify(m => m.Invoke(It.IsAny<IEvent>()), Times.Once());
		}

		[Fact]
		public async Task DoesNotExecuteUnsubscribedHandler() {
			var mockHandler = new Mock<Func<IEvent, Task>>();
			var subId = _aggregator.Register<OtherEvent>(mockHandler.Object);
			_aggregator.Unsubscribe(subId);
			await _aggregator.PublishAsync(new OtherEvent());
			mockHandler.Verify(m => m.Invoke(It.IsAny<IEvent>()), Times.Never());
		}

		[Fact]
		public async Task MultipleSubscriptionsToSameEventWorks() {
			var mockHandler = new Mock<Func<OtherEvent, Task>>();
			_aggregator.Register(mockHandler.Object);
			_aggregator.Register(mockHandler.Object);
			await _aggregator.PublishAsync(new OtherEvent());
			mockHandler.Verify(m => m.Invoke(It.IsAny<OtherEvent>()), Times.Exactly(2));
		}

		[Fact]
		public async Task RemovesCorrectSubscription() {
			var mockHandlerOne = new Mock<Func<IEvent, Task>>();
			var mockHandlerTwo = new Mock<Func<IEvent, Task>>();
			var mockHandlerThree = new Mock<Func<IEvent, Task>>();

			var subIdOne = _aggregator.Register<OtherEvent>(mockHandlerOne.Object);
			var subIdTwo = _aggregator.Register<OtherEvent>(mockHandlerTwo.Object);
			var subIdThree = _aggregator.Register<OtherEvent>(mockHandlerThree.Object);

			_aggregator.Unsubscribe(subIdTwo);
			await _aggregator.PublishAsync(new OtherEvent());

			mockHandlerOne.Verify(m => m.Invoke(It.IsAny<IEvent>()), Times.Once());
			mockHandlerTwo.Verify(m => m.Invoke(It.IsAny<IEvent>()), Times.Never());
			mockHandlerThree.Verify(m => m.Invoke(It.IsAny<IEvent>()), Times.Once());
		}

		private class TestEvent : IEvent {
			public int SomeNumber { get; set; }
		}

		public class OtherEvent : IEvent { }

    }

}
