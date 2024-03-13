using System;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

using VHToolkit.Redirection;
using VHToolkit.Redirection.WorldRedirection;

namespace VHToolkit.Logging {


	[Serializable]
	struct WorldRedirectionData {
		[SerializeField] public float overTime, rotational, curvature;
		[SerializeField] public float overTimeSum, rotationalSum, curvatureSum;
		[SerializeField] float time;
		[SerializeField] float[] maxSums;

		public void AddTo(float overTime, float rotational, float curvature, float time) {
			if (this.maxSums is null || this.maxSums.Length != 3) {
				this.maxSums = new float[3];
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
	public class Socket : MonoBehaviour {
		private Scene scene;
		private WorldRedirection script;
		private DateTime startTime;

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

			Debug.Log($"Launch visualizer with Python {pythonPath}");
			if (filename is null || pythonPath is null || !filename.EndsWith(".py") || !pythonPath.EndsWith(".exe")) return;
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
				Thread thread = new(() => SendMessage(client, redirectionData));
				thread.Start();
				redirectionData.Reset();
				// redirectionData.overTime = 0f;
				// redirectionData.rotational = 0f;
				// redirectionData.curvature = 0f;
			}
		}

		private void SendMessage(TcpClient client, WorldRedirectionData redirectionData) {
			string json = JsonUtility.ToJson(redirectionData);
			// Translate the passed message into ASCII and store it as a Byte array.
			Byte[] messageBytes = System.Text.Encoding.ASCII.GetBytes(json + '\n');

			// Get a client stream for reading and writing.
			NetworkStream stream = client.GetStream();

			try {
				// Send the message to the connected TcpServer.
				stream.Write(messageBytes, 0, messageBytes.Length);
				stream.Flush();
			}
			catch (SocketException) { Debug.LogWarning("Socket closed."); }
		}

		private void Update() {
			redirectionData.AddTo((script.redirect && scene.enableHybridOverTime) ? Razzaque2001OverTimeRotation.GetRedirection(scene) : 0f,
								  (script.redirect && scene.enableHybridRotational) ? Razzaque2001Rotational.GetRedirection(scene) : 0f,
								  (script.redirect && scene.enableHybridCurvature) ? Razzaque2001Curvature.GetRedirection(scene) : 0f,
								  (float)(DateTime.Now - startTime).TotalSeconds);
		}
	}
}
