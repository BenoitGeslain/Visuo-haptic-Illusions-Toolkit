using System;
using System.Collections.Generic;
using System.IO;
using System.Globalization;

using CsvHelper;
using CsvHelper.Configuration;

using UnityEngine;

using VHToolkit.Redirection;
using VHToolkit.Redirection.BodyRedirection;
using VHToolkit.Redirection.WorldRedirection;

namespace VHToolkit.Logging
{
    /// <summary>
    /// Class specifying loggable data for a redirection scene.
    /// </summary>
    public record RedirectionData {
		public DateTime timeStamp = DateTime.Now;
		public string Technique => script switch {
			WorldRedirection => (script as WorldRedirection).Technique.ToString(),
			BodyRedirection => (script as BodyRedirection).Technique.ToString(),
			_ => ""
		};

		public Interaction script;
		public int physicalLimbIndex, virtualLimbIndex;

		public Transform physicalLimb, virtualLimb;

		public RedirectionData(Interaction script, int physicalLimbIndex, int virtualLimbIndex) {
			this.script = script;
			this.physicalLimbIndex = physicalLimbIndex;
			this.virtualLimbIndex = virtualLimbIndex;
			this.physicalLimb = script.scene.limbs[physicalLimbIndex].physicalLimb;
			this.virtualLimb = script.scene.limbs[physicalLimbIndex].virtualLimb[virtualLimbIndex];
		}
	}

	/// <summary>
	/// Helper class for redirection logging.
	/// </summary>
	public sealed class RedirectionDataMap : ClassMap<RedirectionData> {
		/// <summary>
		/// Helper class for logging <c>Scene</c> objects.
		/// </summary>
		public sealed class SceneClassMap : ClassMap<Scene> {
			public SceneClassMap() {
				// Warning! Non-logged attributes in Scene MUST be ignored ([Ignore])
				// AutoMap(new CsvConfiguration(CultureInfo.InvariantCulture) {MemberTypes = MemberTypes.Fields | MemberTypes.Properties | MemberTypes.None});

				/* 	Map(m => m.physicalTarget.position).Name("PhysicalTargetPosition");
					Map(m => m.physicalTarget.rotation).Name("PhysicalTargetOrientation");
					Map(m => m.physicalTarget.rotation.eulerAngles).Name("PhysicalTargetOrientationEuler");
					Map(m => m.virtualTarget.position).Name("VirtualTargetPosition");
					Map(m => m.virtualTarget.rotation).Name("VirtualTargetOrientation");
					Map(m => m.origin.position).Name("OriginPosition"); */
			}
		}
		/// <summary>
		/// Helper class for logging <c>Interaction</c> objects.
		/// </summary>
		public sealed class InteractionClassMap : ClassMap<Interaction> {
			public InteractionClassMap() => References<SceneClassMap>(m => m.scene);
		}
		public RedirectionDataMap() {
			Map(m => m.timeStamp).TypeConverterOption.Format("yyyy/MM/dd-HH:mm:ss.fff").Index(0).Name("TimeStamp");
			Map(m => m.Technique).Index(1).Name("Technique");
			Map(m => m.physicalLimbIndex).Index(2).Name("Physical index");
			Map(m => m.virtualLimbIndex).Index(3).Name("Virtual index");
			Map(m => m.physicalLimb.position).Name("Physical limb position");
			Map(m => m.physicalLimb.rotation).Name("Physical limb orientation");
			Map(m => m.virtualLimb.position).Name("Virtual limb position");
			Map(m => m.virtualLimb.rotation).Name("Virtual limb orientation");
			References<InteractionClassMap>(m => m.script);
		}
	}
	/// <summary>
	/// This class handles the CSV logging behavior at execution time.
	/// </summary>
	[AddComponentMenu("CSV Logging (Script)")]
	public class Logging : Logger<RedirectionData> {
		private string fileName;
		private readonly CsvConfiguration config = new(CultureInfo.InvariantCulture) { HasHeaderRecord = false, MemberTypes = MemberTypes.Fields };


		private void Start() {
			CreateNewFile(logDirectoryPath, optionalFilenamePrefix ?? "");
			script = GetComponent<Interaction>();
		}

		private void Update() {
			for (int i = 0; i < script.scene.limbs.Count; i++) {
				for (int j = 0; j < script.scene.limbs[i].virtualLimb.Count; j++) {
					records.Enqueue(new RedirectionData(script, i, j));
				}
			}
			WriteRecords(records);
		}

		public void CreateNewFile(string logDirectoryPath = null, string optionalFilenamePrefix = null) {
			logDirectoryPath ??= this.logDirectoryPath;
			optionalFilenamePrefix ??= this.optionalFilenamePrefix;
			Debug.Log("Create log file.");
			Directory.CreateDirectory(logDirectoryPath);
			fileName = $"{logDirectoryPath}{optionalFilenamePrefix}{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.csv";

			var observer = new FileObserver(fileName, config);
			observer.Subscribe(this);
			observers.Add(observer);
		}
		private sealed class FileObserver : IObserver<RedirectionData> {
			private StreamWriter writer;
			private CsvWriter csvWriter;
			private IDisposable unsubscriber;
			public void OnCompleted() {
				unsubscriber.Dispose();
				csvWriter.Dispose();
				writer.Dispose();
			}

			public void Subscribe(IObservable<RedirectionData> observable) => unsubscriber = observable?.Subscribe(this);

			public void OnError(Exception error) => Debug.LogError(error);

			public void OnNext(RedirectionData value) {
				csvWriter.Context.RegisterClassMap<RedirectionDataMap>();
				csvWriter.WriteRecord(value);
				csvWriter.NextRecord();
			}

			public FileObserver(string filename, CsvConfiguration config) {
				writer = new StreamWriter(filename, append: true);
				csvWriter = new(writer, configuration: config);
				csvWriter.Context.RegisterClassMap<RedirectionDataMap>();
				csvWriter.WriteHeader<RedirectionData>();
				csvWriter.NextRecord();
			}
		}
	}
}
