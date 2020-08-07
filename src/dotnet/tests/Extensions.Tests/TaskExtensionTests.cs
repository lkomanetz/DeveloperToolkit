using Dtk.Extensions;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Extensions.Tests {

    public class TaskExtensionTests {

        [Fact]
        public void OnFinallyCalledEvenIfExceptionThrown() {
			int timesCalled = 0;
			Task.Run(() => throw new Exception())
				.FireAndForgetSafe((e) => { }, () => Assert.Equal(1, ++timesCalled));
        }

		[Fact]
		public void ExceptionHandlerCalledOnException() {
			var exceptionMessage = "uh oh";
			Task.Run(() => throw new Exception(exceptionMessage))
				.FireAndForgetSafe((e) => Assert.Equal(exceptionMessage, e.Message), () => {});
		}

    }

}
