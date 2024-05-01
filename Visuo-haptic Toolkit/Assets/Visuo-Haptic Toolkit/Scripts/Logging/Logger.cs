using System;
using System.Collections.Generic;

using UnityEngine;

using VHToolkit.Redirection;

namespace VHToolkit.Logging {
	public class Logger<T> : MonoBehaviour, IObservable<T> {
		public string logDirectoryPath = "LoggedData\\";
		[SerializeField] protected string optionalFilenamePrefix;
		protected readonly int bufferSize = 10; // number of records kept before writing to disk

		protected readonly Queue<T> records = new();
		protected readonly HashSet<IObserver<T>> observers = new();
		protected Interaction script;

		protected void WriteRecords(Queue<T> records) {
			if (records.Count > bufferSize) {
				foreach (var record in records) {
					foreach (var observer in observers) {
						observer.OnNext(record);
					}
				}
				records.Clear();
			}
		}

		private sealed class Unsubscriber : IDisposable {
			private readonly HashSet<IObserver<T>> _observers;
			private readonly IObserver<T> _observer;

			public Unsubscriber(HashSet<IObserver<T>> observers, IObserver<T> observer) {
				_observers = observers;
				_observer = observer;
			}

			public void Dispose() => _observers.Remove(_observer);
		}

		IDisposable IObservable<T>.Subscribe(IObserver<T> observer) {
			observers.Add(observer);
			return new Unsubscriber(observers, observer);
		}
	}
}