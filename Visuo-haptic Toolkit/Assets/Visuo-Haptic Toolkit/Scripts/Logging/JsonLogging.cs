using System;
using System.Collections.Generic;
using System.IO;

using UnityEngine;

using VHToolkit.Redirection;
using VHToolkit.Redirection.BodyRedirection;
using VHToolkit.Redirection.WorldRedirection;
using Newtonsoft.Json;

namespace VHToolkit.Logging
{	
	public record VirtualLimbData {
		private readonly Transform vlimb;
		public string Orientation => vlimb.rotation.normalized.ToString();
		public string Position => vlimb.position.ToString();

        public VirtualLimbData(Transform vlimb) => this.vlimb = vlimb;
    }
	public record PhysicalLimbData {
		private readonly Limb limb;
		public string Orientation => limb.physicalLimb.rotation.normalized.ToString();
		public string Position => limb.physicalLimb.position.ToString();
		public List<VirtualLimbData> VirtualLimbs => limb.virtualLimb.ConvertAll(vlimb => new VirtualLimbData(vlimb));

		public PhysicalLimbData(Limb l) => limb = l;
	}
	public record JsonRedirectionData
	{
		public readonly DateTime TimeStamp = DateTime.Now;
		public string Technique => script switch
		{
			WorldRedirection => (script as WorldRedirection).Technique.ToString(),
			BodyRedirection => (script as BodyRedirection).Technique.ToString(),
			_ => ""
		};

		private readonly Interaction script;
		public List<PhysicalLimbData> Limbs => script.scene.limbs.ConvertAll(l => new PhysicalLimbData(l));

		public JsonRedirectionData(Interaction script)
		{
			this.script = script;
		}
	}

	/// <summary>
	/// Logs structured data in the JSON Lines format.
	/// </summary>
	public class JsonLogging : MonoBehaviour
	{

		public string pathToFile = "LoggedData\\";
		[SerializeField] private string fileNamePrefix;
		private string fileName;
		private readonly int bufferSize = 10; // number of records kept before writing to disk

		private List<JsonRedirectionData> records = new();

		private Interaction script;

		private void Start()
		{
			CreateNewFile();
			script = GetComponent<Interaction>();
		}

		private void Update()
		{
			void writeRecords<Data>(List<Data> records)
			{
				if (records.Count > bufferSize)
				{
					using var writer = new StreamWriter(fileName, append: true);
					foreach (var record in records) {
						writer.WriteLine(JsonConvert.SerializeObject(record));
					}
					records.Clear();
				}
			}

			records.Add(new JsonRedirectionData(script));
			writeRecords(records);
		}

		public void CreateNewFile()
		{
			Debug.Log("Create Json log file.");
			Directory.CreateDirectory(pathToFile);
			fileName = $"{pathToFile}{fileNamePrefix}{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.jsonl";
			records = new List<JsonRedirectionData>();
		}
	}
}