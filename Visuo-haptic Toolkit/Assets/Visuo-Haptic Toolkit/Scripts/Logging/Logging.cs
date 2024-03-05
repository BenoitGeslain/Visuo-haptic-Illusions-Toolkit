using System;
using System.Collections.Generic;
using System.IO;
using System.Globalization;
using System.Text.Json;

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
	public class Logging : MonoBehaviour {

		public string pathToFile = "LoggedData\\";
		[SerializeField] private string fileNamePrefix;
		private string fileName;
		private readonly int bufferSize = 10; // number of records kept before writing to disk

		private List<RedirectionData> records = new();
        private readonly CsvConfiguration config = new(CultureInfo.InvariantCulture) { HasHeaderRecord = false, MemberTypes = MemberTypes.Fields };

		private Interaction script;

        private void Start() {
			CreateNewFile();
			script = GetComponent<Interaction>();
		}

        private void Update() {
            void writeRecords<Data, DataMap>(List<Data> records) where DataMap : ClassMap<Data> {
                if (records.Count > bufferSize) {
					using var writer = new StreamWriter(fileName, append: true);
					using var csv = new CsvWriter(writer, config);
					csv.Context.RegisterClassMap<DataMap>();
					csv.WriteRecords<Data>(records);
					records.Clear();
				}
            }

  			for (int i = 0; i < script.scene.limbs.Count; i++) {
				for (int j = 0; j < script.scene.limbs[i].virtualLimb.Count; j++) {
					records.Add(new RedirectionData(script, i, j));
				}
			}

			writeRecords<RedirectionData, RedirectionDataMap>(records);
		}

		public void CreateNewFile() {
			Debug.Log("Create log file.");
			Directory.CreateDirectory(pathToFile);
			fileName = $"{pathToFile}{fileNamePrefix}{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.csv";

			void writeHeaders<Data, DataMap>(out List<Data> records) where DataMap : ClassMap<Data> {
                using StreamWriter writer = new(fileName);
                using CsvWriter csv = new(writer, CultureInfo.InvariantCulture);
				records = new List<Data>();
                csv.Context.RegisterClassMap<DataMap>();
				csv.WriteHeader<Data>();
				csv.NextRecord();
            }

			writeHeaders<RedirectionData, RedirectionDataMap>(out records);
		}
    }
}