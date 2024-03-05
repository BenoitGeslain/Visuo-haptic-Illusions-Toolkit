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
using Newtonsoft.Json;

namespace VHToolkit.Logging
{
	public record JsonRedirectionData
	{
		public DateTime timeStamp = DateTime.Now;
		public string Technique => script switch
		{
			WorldRedirection => (script as WorldRedirection).Technique.ToString(),
			BodyRedirection => (script as BodyRedirection).Technique.ToString(),
			_ => ""
		};

		public Interaction script;

		public JsonRedirectionData(Interaction script)
		{
			this.script = script;
		}
	}

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
						// writer.WriteLine(JsonConvert.SerializeObject(record));
					}
					records.Clear();
				}
			}

			records.Add(new JsonRedirectionData(script));
			writeRecords(records);
		}

		public void CreateNewFile()
		{
			fileName = $"{pathToFile}{fileNamePrefix}{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.json";
			using StreamWriter writer = new(fileName);
			using CsvWriter csv = new(writer, CultureInfo.InvariantCulture);
			records = new List<JsonRedirectionData>();
		}
	}
}