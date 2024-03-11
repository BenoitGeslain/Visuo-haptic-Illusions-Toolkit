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
		public string Position => obj.position.ToString();
		public string Orientation => obj.rotation.normalized.ToString();
		public TransformData(Transform obj) => this.obj = obj;
	}

	public record PhysicalLimbData {
		private readonly Limb limb;
		public string Orientation => limb.physicalLimb.rotation.normalized.ToString();
		public string Position => limb.physicalLimb.position.ToString();
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
		public TransformData PhysicalHead => (script.scene.physicalHead == null) ? null : new(script.scene.physicalHead);
		public List<TransformData> Targets => script.scene.targets.ConvertAll(t => new TransformData(t));
		public TransformData PhysicalTarget => (script.scene.physicalTarget == null) ? null : new(script.scene.physicalTarget);
		public TransformData VirtualTarget => (script.scene.virtualTarget == null) ? null : new(script.scene.virtualTarget);
		public string StrategyDirection => script.scene.forwardTarget.ToString();


		public JsonRedirectionData(Interaction script) {
			this.script = script;
		}
	}

	/// <summary>
	/// Logs structured data in the JSON Lines format.
	/// </summary>
	public class JsonLogging : MonoBehaviour {

		public string logDirectoryPath = "LoggedData\\";
		[SerializeField] private string optionalFilenamePrefix;
		private string fileName;
		private readonly int bufferSize = 10; // number of records kept before writing to disk

		private Queue<JsonRedirectionData> records = new();

		private Interaction script;

		private void Start() {
			CreateNewFile();
			script = GetComponent<Interaction>();
		}

		private void Update() {
			void writeRecords<Data>(Queue<Data> records) {
				if (records.Count > bufferSize) {
					using var writer = new StreamWriter(fileName, append: true);
					foreach (var record in records) {
						writer.WriteLine(JsonConvert.SerializeObject(record));
					}
					records.Clear();
				}
			}

			records.Enqueue(new JsonRedirectionData(script));
			writeRecords(records);
		}

		public void CreateNewFile() {
			Directory.CreateDirectory(logDirectoryPath);
			fileName = $"{logDirectoryPath}{optionalFilenamePrefix}{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.jsonl";
			records = new Queue<JsonRedirectionData>();
		}
	}
}