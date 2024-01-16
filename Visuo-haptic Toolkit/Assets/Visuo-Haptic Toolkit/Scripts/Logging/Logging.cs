using System;
using System.Collections.Generic;
using System.IO;
using System.Globalization;
using System.Linq;

using CsvHelper;
using CsvHelper.Configuration;

using UnityEngine;

using VHToolkit.Redirection;

namespace VHToolkit.Logging {

    public record RedirectionData {
        public DateTime timeStamp = DateTime.Now;
        public string Technique => script switch {
            WorldRedirection => (script as WorldRedirection).technique.ToString(),
            BodyRedirection => (script as BodyRedirection).GetTechnique().ToString(),
            _ => ""
        };

        public Interaction script;

        public RedirectionData(Interaction script) {
            this.script = script;
        }
    }

	public sealed class RedirectionDataMap : ClassMap<RedirectionData> {
		public sealed class SceneClassMap : ClassMap<Scene> {
			public SceneClassMap() {
				// Warning! Non-logged attributes in Scene MUST be ignored ([Ignore])
				AutoMap(new CsvConfiguration(CultureInfo.InvariantCulture) {MemberTypes = MemberTypes.Fields | MemberTypes.Properties | MemberTypes.None});
				// Map(m => m.limbs.Select(limb => limb.PhysicalLimb.position).ToList()).Name("PhysicalLimbPosition");
				// Map(m => m.limbs.Select(limb => limb.PhysicalLimb.rotation).ToList()).Name("PhysicalLimbOrientation");
				// Map(m => m.limbs.Select(limb => limb.PhysicalLimb.rotation.eulerAngles).ToList()).Name("PhysicalLimbOrientationEuler");
				// Map(m => m.limbs.SelectMany(limb => limb.VirtualLimb.Select(vlimb => vlimb.position)).ToList()).Name("VirtualLimbPosition");
				// Map(m => m.limbs.SelectMany(limb => limb.VirtualLimb.Select(vlimb => vlimb.rotation)).ToList()).Name("VirtualLimbOrientation");
				// Map(m => m.limbs.SelectMany(limb => limb.VirtualLimb.Select(vlimb => vlimb.rotation.eulerAngles)).ToList()).Name("VirtualLimbOrientationEuler");
				Map(m => m.physicalTarget.position).Name("PhysicalTargetPosition");
				Map(m => m.physicalTarget.rotation).Name("PhysicalTargetOrientation");
				Map(m => m.physicalTarget.rotation.eulerAngles).Name("PhysicalTargetOrientationEuler");
				Map(m => m.virtualTarget.position).Name("VirtualTargetPosition");
				Map(m => m.virtualTarget.rotation).Name("VirtualTargetOrientation");
				Map(m => m.virtualTarget.rotation.eulerAngles).Name("VirtualTargetOrientationEuler");
				Map(m => m.origin.position).Name("OriginPosition");
			}
		}
		public sealed class InteractionClassMap : ClassMap<Interaction> {
			public InteractionClassMap() {
				References<SceneClassMap>(m => m.scene);
			}
		}
		public RedirectionDataMap() {
			Map(m => m.timeStamp).TypeConverterOption.Format("yyyy/MM/dd-HH:mm:ss.fff").Index(0).Name("TimeStamp");
			Map(m => m.Technique).Index(1);
			References<InteractionClassMap>(m => m.script);
		}
	}

	/// <summary>
	/// This class handles the logging behavior at execution time.
	/// </summary>
	public class Logging : MonoBehaviour {

		public string pathToFile = "LoggedData\\";
		[SerializeField] private string fileNamePrefix;
		private string fileName;
		private int bufferSize = 10; // number of records kept before writing to disk

		private List<RedirectionData> records = new();
        private readonly CsvConfiguration config = new(CultureInfo.InvariantCulture) { HasHeaderRecord = false, MemberTypes = MemberTypes.Fields };

		private Interaction script;

        private void Start() {
			createNewFile();
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

			records.Add(new RedirectionData(script));
			writeRecords<RedirectionData, RedirectionDataMap>(records);
		}

		public void createNewFile() {
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