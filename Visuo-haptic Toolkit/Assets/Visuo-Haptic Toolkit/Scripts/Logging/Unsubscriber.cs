using System;
using System.Collections.Generic;

namespace VHToolkit.Logging {
	sealed class HashSetUnsubscriber<T> : IDisposable {
		private readonly HashSet<IObserver<T>> _observers;
		private readonly IObserver<T> _observer;

		public HashSetUnsubscriber(HashSet<IObserver<T>> observers, IObserver<T> observer) {
			_observers = observers;
			_observer = observer;
		}

		public void Dispose() => _observers.Remove(_observer);
	}
}
