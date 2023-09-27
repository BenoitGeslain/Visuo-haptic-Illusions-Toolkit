using System;
using System.Collections.Generic;
using System.IO;
using System.Globalization;

using CsvHelper;
using CsvHelper.Configuration;

using UnityEngine;
using BG.Redirection;
using System.Linq;
using UnityEditor;

namespace BG.Logging {

    public record RedirectionData {
        public DateTime timeStamp = DateTime.Now;
		public BodyRedirection script = (BodyRedirection)Toolkit.Instance.rootScript;
    }

	public sealed class RedirectionDataMap : ClassMap<RedirectionData> {
		public RedirectionDataMap() {
			Map(m => m.timeStamp).TypeConverterOption.Format("yyyy/MM/dd-HH:mm:ss.fff").Index(0).Name("TimeStamp");
			Map(m => m.script.technique).Index(1).Name("Technique");
			Map(m => m.script.scene.physicalHand.position).Index(2).Name("PhysicalHandPosition");
			Map(m => m.script.scene.physicalHand.rotation).Index(3).Name("PhysicalHandOrientation");
			Map(m => m.script.scene.physicalHand.rotation.eulerAngles).Index(3).Name("PhysicalHandOrientationEuler");
			Map(m => m.script.scene.virtualHand.position).Index(4).Name("VirtualHandPosition");
			Map(m => m.script.scene.virtualHand.rotation).Index(5).Name("VirtualHandOrientation");
			Map(m => m.script.scene.virtualHand.rotation.eulerAngles).Index(5).Name("VirtualHandOrientationEuler");
			Map(m => m.script.scene.physicalTarget.position).Index(6).Name("PhysicalTargetPosition");
			Map(m => m.script.scene.physicalTarget.rotation).Index(7).Name("PhysicalTargetOrientation");
			Map(m => m.script.scene.physicalTarget.rotation.eulerAngles).Index(7).Name("PhysicalTargetOrientationEuler");
			Map(m => m.script.scene.virtualTarget.position).Index(8).Name("VirtualTargetPosition");
			Map(m => m.script.scene.virtualTarget.rotation).Index(9).Name("VirtualTargetOrientation");
			Map(m => m.script.scene.virtualTarget.rotation.eulerAngles).Index(9).Name("VirtualTargetOrientationEuler");
			Map(m => m.script.scene.origin.position).Index(10).Name("OriginPosition");
		}
	}

	public class Logging : MonoBehaviour {

		public string pathToFile = "LoggedData\\";
		[SerializeField] private string fileNamePrefix;
		private string fileName;
		private int bufferSize = 10; // number of records kept before writing to disk

		private List<RedirectionData> records = new();
        private readonly CsvConfiguration config = new(CultureInfo.InvariantCulture) { HasHeaderRecord = false };

        private void Start() {
			createNewFile();
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

			records.Add(new RedirectionData());
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
            }

			writeHeaders<RedirectionData, RedirectionDataMap>(out records);
		}

        private string[] targetsToString(Transform[] targets) => targets.Select(t => t.name).ToArray();
    }
}