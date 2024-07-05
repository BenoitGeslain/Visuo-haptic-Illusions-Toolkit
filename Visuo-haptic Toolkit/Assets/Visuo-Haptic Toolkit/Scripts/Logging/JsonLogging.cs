using System;
using System.Collections.Generic;
using System.IO;

using UnityEngine;

using VHToolkit.Redirection;
using VHToolkit.Redirection.BodyRedirection;
using VHToolkit.Redirection.WorldRedirection;
using Newtonsoft.Json;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Linq;
using UnityEditor;

namespace VHToolkit.Logging {

	/// <summary>
	/// Record type <c>TransformData</c> is a wrapper around Unity's <c>Transform</c>.
	/// It allows for serializing the underlying <c>Transform</c>'s <c>position</c> and <c>rotation</c> properties.
	/// </summary>
	public record TransformData {
		private readonly Transform obj;
		public string Position => obj ? obj.position.ToString() : "NULL";
		public string Orientation => obj ? obj.rotation.normalized.ToString() : "NULL";
		public TransformData(Transform obj) => this.obj = obj;
	}

	/// <summary>
	/// Record type <c>PhysicalLimbData</c> is a wrapper around <c>Limb</c>.
	/// It allows for serializing the <c>position</c> and <c>rotation</c> properties of the underlying physical limb,
	/// as well as those of all underlying virtual limbs.
	/// </summary>
	public record PhysicalLimbData {
		private readonly Limb limb;
		public string Position => limb.physicalLimb.position.ToString();
		public string Orientation => limb.physicalLimb.rotation.normalized.ToString();
		public List<TransformData> VirtualLimbs => limb.virtualLimb.ConvertAll(vlimb => new TransformData(vlimb));

		public PhysicalLimbData(Limb l) => limb = l;
	}

    /// <summary>
    /// Record type <c>WorldRedirectionData</c> is a wrapper around <c>WRData</c>.
    /// It allows for serializing the redirection values : rotation over time, rotational and curvature
    /// given by the WorldRedirection script
    /// </summary>
    public record WorldRedirectionData
	 {
		public readonly float RotationOverTime;
        public readonly float Rotational;
		public readonly float Curvature;
		public readonly float Time;

        public WorldRedirectionData(float overTime, float rotational, float curvature, float time)
        {
            this.RotationOverTime = overTime;
            this.Rotational = rotational;
            this.Curvature = curvature;
            this.Time = time;
        }
    }

	public record JsonRedirectionData {
		public readonly DateTime TimeStamp = DateTime.Now;
		private readonly Interaction script;
		private readonly DateTime startTime;

		/// <value>
		/// Property <c>Strategy</c> is a string corresponding to the name of the current World Redirection
		/// target selection strategy, if any, or the empty string.
		/// </value>
		public string Strategy => script switch {
			WorldRedirection wr => wr.strategy.ToString(),
			BodyRedirection => String.Empty,
			_ => String.Empty
		};

		/// <value>
		/// Property <c>Technique</c> is a string corresponding to the name of the current redirection
		/// technique, if any, or the empty string.
		/// </value>
		public string Technique => script switch {
			WorldRedirection wr => wr.Technique.ToString(),
			BodyRedirection br => br.Technique.ToString(),
			_ => String.Empty
		};

		public bool Redirecting => script.redirect;

		public WorldRedirectionData RedirectionData => 
            new WorldRedirectionData((script.redirect && script.scene.enableHybridOverTime) ? Razzaque2001OverTimeRotation.GetRedirection(script.scene) : 0f,
                       (script.redirect && script.scene.enableHybridRotational) ? Razzaque2001Rotational.GetRedirection(script.scene) : 0f,
                       (script.redirect && script.scene.enableHybridCurvature) ? Razzaque2001Curvature.GetRedirection(script.scene) : 0f,
                       (float)(TimeStamp - startTime).TotalSeconds);

		public List<PhysicalLimbData> Limbs => script.scene.limbs.ConvertAll(l => new PhysicalLimbData(l));
		public TransformData PhysicalHead => script.scene.physicalHead ? new(script.scene.physicalHead) : null;
		public List<TransformData> Targets => script.scene.targets.ConvertAll(t => new TransformData(t));
		public TransformData PhysicalTarget => script.scene.physicalTarget ? new(script.scene.physicalTarget) : null;
		public TransformData VirtualTarget => script.scene.virtualTarget ? new(script.scene.virtualTarget) : null;
		public string StrategyDirection => script.scene.forwardTarget != null ? Convert.ToString(script.scene.forwardTarget.ToString()) : null;

		public JsonRedirectionData(Interaction script, DateTime startTime)
		{
			this.script = script;
			this.startTime = startTime;
		}
	}

	/// <summary>
	/// Logs structured data in the JSON Lines format.
	/// </summary>
	public class JsonLogging : Logger<JsonRedirectionData>
	{
		private Scene scene;
        private DateTime startTime;

		[SerializeField] private string filename;

		[SerializeField] private string pythonPath;

		private void Start() {
			CreateNewFile(logDirectoryPath, optionalFilenamePrefix ?? "");
			script = GetComponent<Interaction>();
			scene = script.scene;

			if (script is WorldRedirection)
                CreateTcpClient(filename, pythonPath);

            startTime = DateTime.Now;
        }

		private void Update()
        {
            records.Enqueue(new JsonRedirectionData(script, startTime));
            WriteRecords(records);
        }

        private void OnDestroy()
        {
            while (observers.Count > 0)
            {
                var o = observers.FirstOrDefault();
                o.OnCompleted();
            }
        }

        /// <summary>
        /// Create a new log file.
        /// </summary>
        /// <param name="logDirectoryPath">The path to the directory where the file should be placed.</param>
        /// <param name="optionalFilenamePrefix">An optional prefix string to appear in the filename before its timestamp.</param>
        public void CreateNewFile(string logDirectoryPath, string optionalFilenamePrefix = "") {
			Directory.CreateDirectory(logDirectoryPath);
			var fileName = $"{logDirectoryPath}{optionalFilenamePrefix}{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.jsonl";
			var observer = new JsonFileObserver(fileName);
			observer.Subscribe(this);
		}

        /// <summary>
        /// Create a new TCP client
        /// </summary>
        /// <param name="filename">The path to the python script that starts the server socket locally</param>
        /// <param name="pythonPath">The path to the python executable file</param>
		public void CreateTcpClient(string filename, string pythonPath)
		{
			var wrapper = new TcpSocketObserver(filename, pythonPath);
			wrapper.Subscribe(this);
        }

		/// <summary>
		/// The class <c>JsonFileObserver</c> implements the Observer pattern for <c>JsonRedirectionData</c> instances,
		/// serializing the information which it receives and writing it to a file.
		/// </summary>
		private sealed class JsonFileObserver : AbstractFileObserver<JsonRedirectionData> {
			public JsonFileObserver(string filename) : base(filename) { }

			override public void OnNext(JsonRedirectionData value) => writer.WriteLine(JsonConvert.SerializeObject(value));
		}

        /// <summary>
        /// The class <c>TcpSocketObserver</c> implements the Observer pattern for <c>JsonRedirectionData</c> instances,
        /// serializing the World Redirection information which it receives and sending it over a TCP connection to a local socket.
        /// </summary>
        private sealed class TcpSocketObserver : IObserver<JsonRedirectionData>
        {
            private IDisposable unsubscriber;

            private TcpClient client;
            private CancellationTokenSource periodicConnectTokenSrc, periodicSendMsgTokenSrc;

            private string filename;
            private string pythonPath;

            private WRData currentRedirectionData;

            public TcpSocketObserver(string filename, string pythonPath)
            {
                // TODO move the python process in an entire different wrapper than the Socket observer
                this.filename = filename;
                this.pythonPath = pythonPath;
                periodicConnectTokenSrc = new();
                periodicSendMsgTokenSrc = new();

                currentRedirectionData = new();

                if (filename is null || !filename.EndsWith(".py") || !File.Exists(filename))
                {
                    UnityEngine.Debug.LogWarning($"Invalid Python script {filename}.");
                }
                else if (pythonPath is null || !pythonPath.EndsWith(".exe") || !File.Exists(pythonPath))
                {
                    UnityEngine.Debug.LogWarning($"Invalid Python executable path {filename}.");
                }
                else
                {
                    UnityEngine.Debug.Log($"Launch visualizer with Python {pythonPath}.");
                    // TODO not great for non-windows
                    System.Diagnostics.Process p = new()
                    {
                        StartInfo = new System.Diagnostics.ProcessStartInfo(pythonPath, filename)
                        {
                            ErrorDialog = false,
                            RedirectStandardError = true,
                            RedirectStandardOutput = true,
                            UseShellExecute = false,
                            CreateNoWindow = true,
                        }
                    };

                    // allow for redirecting python process standard output to Unity's console
                    p.ErrorDataReceived += (sendingProcess, errorLine) => UnityEngine.Debug.LogError(errorLine.Data);
                    p.OutputDataReceived += (sendingProcess, dataLine) => UnityEngine.Debug.Log(dataLine.Data);

                    p.Start();
                    p.BeginErrorReadLine();
                    p.BeginOutputReadLine();
                }

                PeriodicAsync(GetClient, TimeSpan.FromSeconds(5), periodicConnectTokenSrc.Token);
                PeriodicAsync(StartSendingMessages, TimeSpan.FromSeconds(1), periodicSendMsgTokenSrc.Token);
            }
            public void Subscribe(IObservable<JsonRedirectionData> observable) => unsubscriber = observable?.Subscribe(this);

            public void OnCompleted()
            {
                periodicConnectTokenSrc.Cancel();
                periodicSendMsgTokenSrc.Cancel();
                client.Close();
                unsubscriber.Dispose();
            }

            public void OnError(Exception error)
            {
                throw new NotImplementedException();
            }

            void IObserver<JsonRedirectionData>.OnNext(JsonRedirectionData value)
            {
                var redirectionData = value.RedirectionData;
                currentRedirectionData.AddTo(redirectionData.RotationOverTime, redirectionData.Rotational,
                                            redirectionData.Curvature, redirectionData.Time);
            }

            private void GetClient()
            {
                client ??= new();
                if (!client.Connected)
                {
                    try
                    {
                        client.ConnectAsync("localhost", 13000);
                    }
                    catch (SocketException) { }
                }
            }
            private void StartSendingMessages()
            {
                if (client != null && client.Connected)
                {
                    string json = JsonConvert.SerializeObject(currentRedirectionData);
                    Thread thread = new(() => SendMessage(client, json));
                    thread.Start();
                    currentRedirectionData.Reset();
                }
            }

            private void SendMessage(TcpClient client, string json)
            {

                // Translate the passed message into ASCII and store it as a Byte array.
                byte[] messageBytes = System.Text.Encoding.ASCII.GetBytes(json + '\n');

                try
                {
                    // Get a client stream for reading and writing.
                    NetworkStream stream = client.GetStream();
                    // Send the message to the connected TcpServer.
                    stream.Write(messageBytes, 0, messageBytes.Length);
                    stream.Flush();
                }
                catch (IOException) { UnityEngine.Debug.LogWarning("Socket closed."); }
            }

            private static async void PeriodicAsync(Action action, TimeSpan interval,
                                                    CancellationToken cancellationToken = default)
            {
                while (true)
                {
                    await Task.Run(action);

                    try
                    {
                        await Task.Delay(interval, cancellationToken);
                    }
                    catch (TaskCanceledException) { break; }
                }
            }
        }

    }
}