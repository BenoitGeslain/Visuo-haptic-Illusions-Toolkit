using System;
using System.Collections.Generic;
using System.IO;

using UnityEngine;

using VHToolkit.Redirection;
using VHToolkit.Redirection.BodyRedirection;
using VHToolkit.Redirection.WorldRedirection;
using Newtonsoft.Json;

namespace VHToolkit.Logging {

	/// <summary>
	/// Record type <c>TransformData</c> is a wrapper around Unity's <c>Transform</c>.
	/// It allows for serializing the underlying <c>Transform</c>'s <c>position</c> and <c>rotation</c> properties.
	/// </summary>
	public record TransformData {
		private readonly Transform obj;
		public string Position => obj ? obj.position.ToString() : "NULL";
		public string Orientation => obj ? obj.rotation.normalized.ToString() : "NULL";
		public TransformData(Transform obj) => this.obj = obj;
	}

	/// <summary>
	/// Record type <c>PhysicalLimbData</c> is a wrapper around <c>Limb</c>.
	/// It allows for serializing the <c>position</c> and <c>rotation</c> properties of the underlying physical limb,
	/// as well as those of all underlying virtual limbs.
	/// </summary>
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

		/// <value>
		/// Property <c>Strategy</c> is a string corresponding to the name of the current World Redirection
		/// target selection strategy, if any, or the empty string.
		/// </value>
		public string Strategy => script switch {
			WorldRedirection => (script as WorldRedirection).strategy.ToString(),
			BodyRedirection => String.Empty,
			_ => String.Empty
		};

		/// <value>
		/// Property <c>Technique</c> is a string corresponding to the name of the current redirection
		/// technique, if any, or the empty string.
		/// </value>
		public string Technique => script switch {
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
		public string StrategyDirection => script.scene.forwardTarget != null ? Convert.ToString(script.scene.forwardTarget.ToString()) : null;

		public JsonRedirectionData(Interaction script) => this.script = script;
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
		/// <summary>
		/// Create a new log file.
		/// </summary>
		/// <param name="logDirectoryPath">The path to the directory where the file should be placed.</param>
		/// <param name="optionalFilenamePrefix">An optional prefix string to appear in the filename before its timestamp.</param>
		public void CreateNewFile(string logDirectoryPath, string optionalFilenamePrefix = "") {
			Directory.CreateDirectory(logDirectoryPath);
			var fileName = $"{logDirectoryPath}{optionalFilenamePrefix}{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.jsonl";
			var observer = new JsonFileObserver(fileName);
			observer.Subscribe(this);
			observers.Add(observer);
		}

		/// <summary>
		/// The class <c>JsonFileObserver</c> implements the Observer pattern for <c>JsonRedirectionData</c> instances,
		/// serializing the information which it receives and writing it to a file.
		/// </summary>
		private sealed class JsonFileObserver : AbstractFileObserver<JsonRedirectionData> {
			public JsonFileObserver(string filename) : base(filename) { }

			override public void OnNext(JsonRedirectionData value) => writer.WriteLine(JsonConvert.SerializeObject(value));
		}
	}
}