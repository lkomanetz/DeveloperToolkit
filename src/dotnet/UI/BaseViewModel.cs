using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Dtk.UI {

	public abstract class BaseViewModel : INotifyPropertyChanged {

		public event PropertyChangedEventHandler PropertyChanged = delegate { };

		public void FirePropertyChanged([CallerMemberName] string propertyName = "") => PropertyChanged(this, new PropertyChangedEventArgs(propertyName));

		public virtual void Dispose() { }

	}

	public static class LinqExtensions {

		public static void SetPropertyValue<T, TValue>(
			this T target,
			Expression<Func<T, TValue>> memberLamda,
			TValue newValue,
			ref TValue oldValue,
			Action<string> onPropertySet
		) where T : BaseViewModel {

			if (!(memberLamda.Body is MemberExpression expression)) return;
			if (!(expression.Member is PropertyInfo property)) return;
			if (property.PropertyType != typeof(TValue)) return;

			if (oldValue == null) {
				oldValue = newValue;
				onPropertySet(property.Name);
				return;
			}

			if (oldValue.Equals(newValue)) return;
			oldValue = newValue;
			onPropertySet(property.Name);
		}

	}

}
