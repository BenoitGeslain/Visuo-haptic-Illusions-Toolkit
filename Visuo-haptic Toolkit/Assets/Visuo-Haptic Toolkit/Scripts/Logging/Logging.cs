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

	public class BodyRedirectionData {
		public DateTime timeStamp = DateTime.Now;
		public BodyRedirection script = (BodyRedirection)Toolkit.Instance.rootScript;

		public BodyRedirectionData(DateTime timeStamp, BodyRedirection script) {
			this.timeStamp = timeStamp;
			this.script = script;
		}
	}


	public sealed class BodyRedirectionDataMap : ClassMap<BodyRedirectionData> {
		public BodyRedirectionDataMap() {
			Map(m => m.timeStamp).TypeConverterOption.Format("yyyy/MM/dd-HH:mm:ss").Name("TimeStamp");
			Map(m => m.script.technique).Name("Technique");
			Map(m => m.script.scene.physicalHand.position).Name("physicalHandPosition");
			Map(m => m.script.scene.physicalHand.rotation).Name("physicalHandOrientation");
			Map(m => m.script.scene.virtualHand.position).Name("virtualHandPosition");
			Map(m => m.script.scene.virtualHand.rotation).Name("virtualHandPosition");
			Map(m => m.script.scene.physicalTarget.position).Name("physicalTargetPosition");
			Map(m => m.script.scene.physicalTarget.rotation).Name("physicalTargetOrientation");
			Map(m => m.script.scene.virtualTarget.position).Name("virtualTargetPosition");
			Map(m => m.script.scene.virtualTarget.rotation).Name("virtualTargetPosition");
			Map(m => m.script.scene.origin.position).Name("originPosition");
		}
	}

	public class Logging : MonoBehaviour {

		public string pathToFile = "LoggedData\\";
		[SerializeField] private string fileNamePrefix;
		private string fileName;

		private List<BodyRedirectionData> recordsBR;
		private List<WorldRedirectionScene> recordsWR;

		private void Start() {
			createNewFile();
		}

		private void Update() {
			if (recordsBR != null) {
				BodyRedirection rootScript = (BodyRedirection) Toolkit.Instance.rootScript;
				recordsBR.Add(new BodyRedirectionData(DateTime.Now, rootScript));
				if(recordsBR.Count > 10) {
					using (var writer = new StreamWriter(fileName, append: true)) {
						var config = new CsvConfiguration(CultureInfo.InvariantCulture) {
							HasHeaderRecord = false,
						};
						using (var csv = new CsvWriter(writer, config)) {
							csv.Context.RegisterClassMap<BodyRedirectionDataMap>();
							csv.WriteRecords<BodyRedirectionData>(recordsBR);
						}
					}
				}
			} else if (recordsWR != null && recordsWR.Count > 10) {
				using (var writer = new StreamWriter(fileName, append: true)) {
					var config = new CsvConfiguration(CultureInfo.InvariantCulture) {
						HasHeaderRecord = false,
					};
					using (var csv = new CsvWriter(writer, config)) {
						csv.WriteRecords(recordsWR.Select(record => record.GetHeadToHeadDistance()));
						recordsWR.Clear();
					}
				}
			}
		}

		public void createNewFile() {
			try {
				BodyRedirection rootScript = (BodyRedirection) Toolkit.Instance.rootScript;
				recordsBR = new List<BodyRedirectionData>();

				fileName = pathToFile + fileNamePrefix + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".csv";
				using (var writer = new StreamWriter(fileName)) {
					using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture)) {
						csv.Context.RegisterClassMap<BodyRedirectionDataMap>();
						csv.WriteRecords<BodyRedirectionData>(recordsBR);
					}
				}
			} catch (InvalidCastException) {
				try {
					WorldRedirection rootScript = (WorldRedirection) Toolkit.Instance.rootScript;
					recordsWR = new List<WorldRedirectionScene>();

					fileName = pathToFile + fileNamePrefix + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".csv";
					using (var writer = new StreamWriter(fileName)) {
						using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))	// TODO create a configuration to not write the header
							csv.WriteRecords(recordsWR);
					}
				} catch (InvalidCastException) {
					Debug.LogError("Could not cast the toolkit rootsctipt to a known type.");
				}
			}
		}

		private string[] targetsToString(Transform[] targets) {
			string[] s = new string[targets.Length];
			for (var i=0; i<targets.Length; i++) {
				s[i] = targets[i].name;
			}
			return s;
		}
	}
}