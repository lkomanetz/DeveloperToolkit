using Dtk.UI;
using System;
using System.Threading.Tasks;
using Xunit;

namespace UI.Tests {

    public class ViewModelTests {

        [Fact]
        public async Task IsWaitingPropertyChanges() {
			var callCount = 0;
			var vm = new TestViewModel();
			vm.PropertyChanged += (sender, args) => {
				if (args.PropertyName == "IsWaiting") ++callCount;
				if (callCount == 1) Assert.True(vm.IsWaiting);
				if (callCount == 2) Assert.False(vm.IsWaiting);
			};
			await vm.TestMethodAsync();
        }

		[Fact]
		public void SetPropertyValueUpdatesPrivateMember() {
			var vm = new TestViewModel();
			Assert.Equal(0, vm.SomeNumber);
            vm.SomeNumber = 1;
            Assert.Equal(1, vm.SomeNumber);
            vm.SomeNumber = 2;
            Assert.Equal(2, vm.SomeNumber);
		}

		private class TestViewModel : ViewModelWithWait {

			public Task TestMethodAsync() => DoWhileWaitingAsync(() => Task.CompletedTask);

			private int _someNumber;
			public int SomeNumber {
				get => _someNumber;
				set => this.SetProperty(value, ref _someNumber);
			}

		}
    }

}
