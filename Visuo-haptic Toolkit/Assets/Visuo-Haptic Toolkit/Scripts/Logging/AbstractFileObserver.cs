using System;
using System.IO;
using UnityEngine;

namespace VHToolkit.Logging {
	abstract class AbstractFileObserver<T> : IObserver<T> {
		protected readonly StreamWriter writer;
		private IDisposable unsubscriber;
		public void OnCompleted() {
			unsubscriber.Dispose();
			writer.Dispose();
		}

		public void Subscribe(IObservable<T> observable) => unsubscriber = observable?.Subscribe(this);

		public void OnError(Exception error) => Debug.LogError(error);

		abstract public void OnNext(T value);

		public AbstractFileObserver(string filename) {
			writer = new StreamWriter(filename, append: true);
		}
	}
}