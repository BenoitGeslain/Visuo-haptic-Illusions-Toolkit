using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

using VHToolkit.Redirection;
using VHToolkit.Redirection.WorldRedirection;

namespace VHToolkit.Logging {


	[Serializable]
	public struct WorldRedirectionData {
		[SerializeField] public float overTime, rotational, curvature;
		[SerializeField] public float overTimeSum, rotationalSum, curvatureSum;
		[SerializeField] float time;
		[SerializeField] float[] maxSums;

		public void AddTo(float overTime, float rotational, float curvature, float time) {
			if (maxSums?.Length != 3) {
				maxSums = new float[3];
			}
			this.overTime += Mathf.Abs(overTime);
			this.rotational += Mathf.Abs(rotational);
			this.curvature += Mathf.Abs(curvature);
			var tmp = new float[] { overTime, rotational, curvature }.ToList().ConvertAll(Mathf.Abs);
			var m = tmp.Max();
			var maxComponentIndex = tmp.IndexOf(m);

			this.overTimeSum += Mathf.Abs(overTime);
			this.rotationalSum += Mathf.Abs(rotational);
			this.curvatureSum += Mathf.Abs(curvature);
			this.maxSums[maxComponentIndex] += m;
			this.time = time;
		}

		public void Reset() {
			this.overTime = 0f;
			this.rotational = 0f;
			this.curvature = 0f;
		}
	}
	public class VisualizerWrapper : IObserver<WorldRedirectionData> {
		private string filename;
		private string pythonPath;
		private IDisposable unsubscriber;

		public void Subscribe(IObservable<WorldRedirectionData> observable) => unsubscriber = observable?.Subscribe(this);

		public VisualizerWrapper(string filename, string pythonPath) {
			this.filename = filename;
			this.pythonPath = pythonPath;
			if (filename is null || !filename.EndsWith(".py") || !File.Exists(filename)) {
				Debug.LogWarning($"Invalid Python script {filename}.");
			}
			else if (pythonPath is null || !pythonPath.EndsWith(".exe") || !File.Exists(pythonPath)) {
				Debug.LogWarning($"Invalid Python executable path {filename}.");
			}
			else {
				Debug.Log($"Launch visualizer with Python {pythonPath}.");
				// TODO not great for non-windows
				System.Diagnostics.Process p = new() {
					StartInfo = new System.Diagnostics.ProcessStartInfo(pythonPath, filename) {
						RedirectStandardOutput = true,
						UseShellExecute = false,
						CreateNoWindow = true
					}
				};
				p.Start();
			}
		}

		public void OnCompleted() {
			throw new NotImplementedException();
		}

		public void OnError(Exception error) {
			throw new NotImplementedException();
		}

		void IObserver<WorldRedirectionData>.OnNext(WorldRedirectionData value) {
			throw new NotImplementedException();
		}
	}
	public class Socket : MonoBehaviour, IObservable<WorldRedirectionData> {
		private Scene scene;
		private WorldRedirection script;
		private DateTime startTime;


		private readonly HashSet<IObserver<WorldRedirectionData>> observers = new();

		private TcpClient client;

		[Tooltip("File name for the Python visualization script.")]
		[SerializeField] private string filename;

		[Tooltip("File name for the Python executable path.")]
		[SerializeField] private string pythonPath;

		private Razzaque2001Hybrid loggingTechnique;

		private WorldRedirectionData redirectionData;

		private void Start() {
			script = GetComponent<WorldRedirection>();
			scene = script.scene;
			loggingTechnique = new();
			redirectionData = new();
			client = new();
			LaunchVisualizer();
			InvokeRepeating(nameof(GetClient), 0f, 5f);
			InvokeRepeating(nameof(StartSendingMessages), 1f, 1f);
		}

		public void LaunchVisualizer() {
			var wrapper = new VisualizerWrapper(filename, pythonPath);
			wrapper.Subscribe(this);
		}

		private void GetClient() {
			client ??= new();
			if (!client.Connected) {
				try {
					client.ConnectAsync("localhost", 13000);
					startTime = DateTime.Now;
				}
				catch (SocketException) { }
			}
		}
		private void StartSendingMessages() {
			if (client != null && client.Connected) {
				string json = JsonUtility.ToJson(redirectionData);
				Thread thread = new(() => SendMessage(client, json));
				thread.Start();
				redirectionData.Reset();
			}
		}

		private void SendMessage(TcpClient client, string json) {

			// Translate the passed message into ASCII and store it as a Byte array.
			byte[] messageBytes = System.Text.Encoding.ASCII.GetBytes(json + '\n');

			try {
				// Get a client stream for reading and writing.
				NetworkStream stream = client.GetStream();
				// Send the message to the connected TcpServer.
				stream.Write(messageBytes, 0, messageBytes.Length);
				stream.Flush();
			}
			catch (IOException) { Debug.LogWarning("Socket closed."); }
		}

		private void Update() {
			redirectionData.AddTo((script.redirect && scene.enableHybridOverTime) ? Razzaque2001OverTimeRotation.GetRedirection(scene) : 0f,
								  (script.redirect && scene.enableHybridRotational) ? Razzaque2001Rotational.GetRedirection(scene) : 0f,
								  (script.redirect && scene.enableHybridCurvature) ? Razzaque2001Curvature.GetRedirection(scene) : 0f,
								  (float)(DateTime.Now - startTime).TotalSeconds);
		}

		public IDisposable Subscribe(IObserver<WorldRedirectionData> observer) {
			observers.Add(observer);
			return new HashSetUnsubscriber<WorldRedirectionData>(observers, observer);
		}
	}
}
