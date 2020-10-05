using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Dtk.UI {

	public abstract class BaseViewModel : INotifyPropertyChanged, IDisposable {

		public event PropertyChangedEventHandler PropertyChanged = delegate { };

		protected void FirePropertyChanged([CallerMemberName] string propertyName = "") => PropertyChanged(this, new PropertyChangedEventArgs(propertyName));

		protected void SetProperty<TValue>(TValue source, ref TValue destination, [CallerMemberName] string propertyName = "") {
			if (AreEqual(source, destination)) return;
			destination = source;
			FirePropertyChanged(propertyName);

			static bool AreEqual(TValue left, TValue right) =>
				EqualityComparer<TValue>.Default.Equals(left, right);
        }

		public virtual void Dispose() { }

	}

}
