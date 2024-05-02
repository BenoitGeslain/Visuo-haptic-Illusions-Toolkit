using System;
using System.Collections.Generic;

using UnityEngine;

using VHToolkit.Redirection;

namespace VHToolkit.Logging {
	public partial class Logger<T> : MonoBehaviour, IObservable<T> {
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

		IDisposable IObservable<T>.Subscribe(IObserver<T> observer) {
			observers.Add(observer);
			return new HashSetUnsubscriber<T>(observers, observer);
		}
	}
}