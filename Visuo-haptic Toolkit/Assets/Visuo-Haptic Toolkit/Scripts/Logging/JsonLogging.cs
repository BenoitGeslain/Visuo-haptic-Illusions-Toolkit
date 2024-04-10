using System;
using System.Collections.Generic;
using System.IO;

using UnityEngine;

using VHToolkit.Redirection;
using VHToolkit.Redirection.BodyRedirection;
using VHToolkit.Redirection.WorldRedirection;
using Newtonsoft.Json;

namespace VHToolkit.Logging {
	public record TransformData {
		private readonly Transform obj;
		public string Position => obj ? obj.position.ToString() : "NULL";
		public string Orientation => obj ? obj.rotation.normalized.ToString() : "NULL";
		public TransformData(Transform obj) => this.obj = obj;
	}

	public record PhysicalLimbData {
		private readonly Limb limb;
		public string Position => limb.physicalLimb.position.ToString();
		public string Orientation => limb.physicalLimb.rotation.normalized.ToString();
		public List<TransformData> VirtualLimbs => limb.virtualLimb.ConvertAll(vlimb => new TransformData(vlimb));

		public PhysicalLimbData(Limb l) => limb = l;
	}

	public record JsonRedirectionData {
		public readonly DateTime TimeStamp = DateTime.Now;
		private readonly Interaction script;

		public string Technique => script switch {
			WorldRedirection => (script as WorldRedirection).strategy.ToString(),
			BodyRedirection => "",
			_ => ""
		};

		public string Strategy => script switch {
			WorldRedirection => (script as WorldRedirection).Technique.ToString(),
			BodyRedirection => (script as BodyRedirection).Technique.ToString(),
			_ => ""
		};
		public bool Redirecting => script.redirect;

		public List<PhysicalLimbData> Limbs => script.scene.limbs.ConvertAll(l => new PhysicalLimbData(l));
		public TransformData PhysicalHead => script.scene.physicalHead ? new(script.scene.physicalHead) : null;
		public List<TransformData> Targets => script.scene.targets.ConvertAll(t => new TransformData(t));
		public TransformData PhysicalTarget => script.scene.physicalTarget ? new(script.scene.physicalTarget) : null;
		public TransformData VirtualTarget => script.scene.virtualTarget ? new(script.scene.virtualTarget) : null;
		public string StrategyDirection => script.scene.forwardTarget != null ? script.scene.forwardTarget.ToString() : null;

		public JsonRedirectionData(Interaction script) => this.script = script;
	}

	public class Logger<T> : MonoBehaviour, IObservable<T> {
		public string logDirectoryPath = "LoggedData\\";
		[SerializeField] protected string optionalFilenamePrefix;
		protected readonly int bufferSize = 10; // number of records kept before writing to disk

		protected Queue<T> records = new();
		protected HashSet<IObserver<T>> observers = new();
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
			private HashSet<IObserver<T>> _observers;
			private IObserver<T> _observer;

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

	/// <summary>
	/// Logs structured data in the JSON Lines format.
	/// </summary>
	public class JsonLogging : Logger<JsonRedirectionData> {

		private void Start() {
			CreateNewFile(logDirectoryPath, optionalFilenamePrefix ?? "");
			script = GetComponent<Interaction>();
		}

		private void Update() {
			records.Enqueue(new JsonRedirectionData(script));
			WriteRecords(records);
		}

		public void CreateNewFile(string logDirectoryPath, string optionalFilenamePrefix = "") {
			Directory.CreateDirectory(logDirectoryPath);
			var fileName = $"{logDirectoryPath}{optionalFilenamePrefix}{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.jsonl";
			var observer = new FileObserver(fileName);
			observer.Subscribe(this);
			observers.Add(observer);
		}


		private sealed class FileObserver : IObserver<JsonRedirectionData> {
			private readonly StreamWriter writer;
			private IDisposable unsubscriber;
			public void OnCompleted() {
				unsubscriber.Dispose();
				writer.Dispose();
			}

			public void Subscribe(IObservable<JsonRedirectionData> observable) => unsubscriber = observable?.Subscribe(this);

			public void OnError(Exception error) => Debug.LogError(error);

			public void OnNext(JsonRedirectionData value) => writer.WriteLine(JsonConvert.SerializeObject(value));

			public FileObserver(string filename) => writer = new StreamWriter(filename, append: true);
		}
	}
}