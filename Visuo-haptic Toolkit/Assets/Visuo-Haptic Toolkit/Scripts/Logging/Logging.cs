using System;
using System.Collections.Generic;
using System.IO;
using System.Globalization;

using CsvHelper;

using UnityEngine;
using BG.Redirection;
using System.Runtime.CompilerServices;

namespace BG.Logging {
	public class Data {
		public string technique { get; set; }
		public string strategy { get; set; }

		public Vector3 physicalHandPosition { get; set; }
		public Quaternion physicalHandOrientation { get; set; }
		public Vector3 virtualHandPosition { get; set; }
		public Quaternion virtualHandOrientation { get; set; }

		public Vector3 physicalHeadPosition { get; set; }
		public Quaternion physicalHeadOrientation { get; set; }
		public Vector3 virtualHeadPosition { get; set; }
		public Quaternion virtualHeadOrientation { get; set; }

		public Vector3 physicalTargetPosition { get; set; }
		public Quaternion physicalTargetOrientation { get; set; }
		public Vector3 virtualTargetPosition { get; set; }
		public Quaternion virtualTargetOrientation { get; set; }

		public string[] targets { get; set; }
		public float radius { get; set; }
	}

	public class DataBR {
		public string technique { get; set; }

		public Vector3 physicalHandPosition { get; set; }
		public Quaternion physicalHandOrientation { get; set; }
		public Vector3 virtualHandPosition { get; set; }
		public Quaternion virtualHandOrientation { get; set; }

		public Vector3 physicalTargetPosition { get; set; }
		public Quaternion physicalTargetOrientation { get; set; }
		public Vector3 virtualTargetPosition { get; set; }
		public Quaternion virtualTargetOrientation { get; set; }
	}

	public class DataWR {
		public string technique { get; set; }
		public string strategy { get; set; }

		public Vector3 physicalHeadPosition { get; set; }
		public Quaternion physicalHeadOrientation { get; set; }
		public Vector3 virtualHeadPosition { get; set; }
		public Quaternion virtualHeadOrientation { get; set; }

		public string[] targets { get; set; }
		public float radius { get; set; }
	}

	public class Logging : MonoBehaviour {

		public string pathToFile = "LoggedData\\";
		private string fileName;

		[SerializeField] private string fileNamePrefix;

		// Add button to save to a new file

		private List<DataBR> recordsBR;
		private List<DataWR> recordsWR;

		private void Start() {
			createNewFile();
		}

		private void Update() {
			recordData();

			if (recordsBR != null && recordsBR.Count > 10) {
				using (var writer = new StreamWriter(fileName, append: true)) {
					using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture)) {
						csv.WriteRecords(recordsBR);
						recordsBR.Clear();
					}
				}
			} else if (recordsWR != null && recordsWR.Count > 10) {
				using (var writer = new StreamWriter(fileName, append: true)) {
					using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture)) {
						csv.WriteRecords(recordsWR);
						recordsWR.Clear();
					}
				}
			}
		}

		public void createNewFile() {
			try {
				BodyRedirection rootScript = (BodyRedirection) Toolkit.Instance.rootScript;
				recordsBR = new List<DataBR>();

				fileName = pathToFile + fileNamePrefix + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".csv";
				using (var writer = new StreamWriter(fileName)) {
					using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))	// TODO create a configuration to not write the header
						csv.WriteRecords(recordsBR);
				}
			} catch (InvalidCastException) {
				try {
					WorldRedirection rootScript = (WorldRedirection) Toolkit.Instance.rootScript;
					recordsWR = new List<DataWR>();

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

		private void recordData() {
			if (recordsBR != null) {
				BodyRedirection rootScript = (BodyRedirection) Toolkit.Instance.rootScript;

				var r = new DataBR();

				r.technique = Enum.GetName(typeof(BRTechnique), (int)rootScript.technique);
				r.physicalTargetPosition = rootScript.physicalTarget.position;
				r.physicalTargetOrientation = rootScript.physicalTarget.rotation;
				r.virtualTargetPosition = rootScript.virtualTarget.position;
				r.virtualTargetOrientation = rootScript.virtualTarget.rotation;

				r.physicalHandPosition = rootScript.physicalHand.position;
				r.physicalHandOrientation = rootScript.physicalHand.rotation;
				r.virtualHandPosition = rootScript.virtualHand.position;
				r.virtualHandOrientation = rootScript.virtualHand.rotation;

				recordsBR.Add(r);
			} else if (recordsWR != null) {
				WorldRedirection rootScript = (WorldRedirection) Toolkit.Instance.rootScript;

				var r = new DataWR();

				r.technique = Enum.GetName(typeof(WRTechnique), rootScript.technique);
				r.strategy = Enum.GetName(typeof(WRStrategy), rootScript.strategy);
				r.targets = targetsToString(rootScript.strategyInstance.targets);
				r.radius = rootScript.strategyInstance.radius;

				r.physicalHeadPosition = rootScript.physicalHead.position;
				r.physicalHeadOrientation = rootScript.physicalHead.rotation;
				r.virtualHeadPosition = rootScript.virtualHead.position;
				r.virtualHeadOrientation = rootScript.virtualHead.rotation;

				recordsWR.Add(r);
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