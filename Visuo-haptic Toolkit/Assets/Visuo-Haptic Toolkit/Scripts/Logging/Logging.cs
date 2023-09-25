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

    public abstract record RedirectionData {
        public DateTime timeStamp = DateTime.Now;
    }

    public record BodyRedirectionData : RedirectionData {
		public BodyRedirection script = (BodyRedirection)Toolkit.Instance.rootScript;
    }

	public sealed class BodyRedirectionDataMap : ClassMap<BodyRedirectionData> {
		public BodyRedirectionDataMap() {
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

	public record WorldRedirectionData : RedirectionData {
		public WorldRedirection script = (WorldRedirection)Toolkit.Instance.rootScript;
    }

	public sealed class WorldRedirectionDataMap : ClassMap<WorldRedirectionData> {
		public WorldRedirectionDataMap() {
			Map(m => m.timeStamp).TypeConverterOption.Format("yyyy/MM/dd-HH:mm:ss.fff").Index(0).Name("TimeStamp");
			Map(m => m.script.technique).Index(1).Name("Technique");
			Map(m => m.script.scene.physicalHead.position).Index(2).Name("PhysicalHeadPosition");
			Map(m => m.script.scene.physicalHead.rotation).Index(3).Name("PhysicalHeadOrientation");
			Map(m => m.script.scene.physicalHead.rotation.eulerAngles).Index(3).Name("PhysicalHeadOrientationEuler");
			Map(m => m.script.scene.virtualHead.position).Index(4).Name("VirtualHeadPosition");
			Map(m => m.script.scene.virtualHead.rotation).Index(5).Name("VirtualHeadOrientation");
			Map(m => m.script.scene.virtualHead.rotation.eulerAngles).Index(5).Name("VirtualHeadOrientationEuler");
			Map(m => m.script.scene.forwardTarget).Index(6).Name("ForwardTarget");
			Map(m => m.script.scene.targets).Index(7).Name("Targets");
			Map(m => m.script.scene.radius).Index(8).Name("Radius");
		}
	}

	public class Logging : MonoBehaviour {

		public string pathToFile = "LoggedData\\";
		[SerializeField] private string fileNamePrefix;
		private string fileName;
		private int bufferSize = 10; // number of records kept before writing to disk

		private List<BodyRedirectionData> recordsBR = new();
		private List<WorldRedirectionData> recordsWR = new();

        private readonly CsvConfiguration config = new(CultureInfo.InvariantCulture) { HasHeaderRecord = false };

        private void Start() {
			createNewFile();
			Application.targetFrameRate = 60;
		}

		private void Update() {
            void writeRecords<Data, DataMap>(List<Data> records) where DataMap : ClassMap<Data> {
                if (records.Count > bufferSize) {
					using var writer = new StreamWriter(fileName);
					using var csv = new CsvWriter(writer, config);
					csv.Context.RegisterClassMap<DataMap>();
					csv.WriteRecords<Data>(records);
					records.Clear();
				}
            }
            var script = Toolkit.Instance.rootScript;
            if (script is BodyRedirection) {
                recordsBR.Add(new BodyRedirectionData());
                writeRecords<BodyRedirectionData, BodyRedirectionDataMap>(recordsBR);
            }
            else if (script is WorldRedirection) {
                recordsWR.Add(new WorldRedirectionData());
                writeRecords<WorldRedirectionData, WorldRedirectionDataMap>(recordsWR);
            }
            else Debug.LogError("Could not cast the toolkit rootscript to a known type.");
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

			var script = Toolkit.Instance.rootScript;
            if (script is BodyRedirection) {
                writeHeaders<BodyRedirectionData, BodyRedirectionDataMap>(out recordsBR);
            } else if (script is WorldRedirection) {
                writeHeaders<WorldRedirectionData, WorldRedirectionDataMap>(out recordsWR);
            } else Debug.LogError("Could not cast the toolkit rootscript to a known type.");
		}

        private string[] targetsToString(Transform[] targets) => targets.Select(t => t.name).ToArray();
    }
}