using System;
using System.Collections.Generic;
using System.IO;
using System.Globalization;

using CsvHelper;
using CsvHelper.Configuration;

using UnityEngine;

using VHToolkit.Redirection;
using Newtonsoft.Json;

namespace VHToolkit.Logging
{
    public class JsonLogging : MonoBehaviour
	{

		public string pathToFile = "LoggedData\\";
		[SerializeField] private string fileNamePrefix;
		private string fileName;
		private readonly int bufferSize = 10; // number of records kept before writing to disk

		private List<RedirectionData> records = new();

		private Interaction script;

		private void Start()
		{
			CreateNewFile();
			script = GetComponent<Interaction>();
		}

		private void Update()
		{
			void writeRecords<Data, DataMap>(List<Data> records) where DataMap : ClassMap<Data>
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

			records.Add(new RedirectionData(script));
			writeRecords<RedirectionData, RedirectionDataMap>(records);
		}

		public void CreateNewFile()
		{
			fileName = $"{pathToFile}{fileNamePrefix}{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.json";
			using StreamWriter writer = new(fileName);
			using CsvWriter csv = new(writer, CultureInfo.InvariantCulture);
			records = new List<RedirectionData>();
		}
	}
}