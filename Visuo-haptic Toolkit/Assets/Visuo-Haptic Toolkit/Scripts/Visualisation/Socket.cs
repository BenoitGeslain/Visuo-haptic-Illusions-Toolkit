using System;
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

		public void AddTo(float overTime, float rotational, float curvature, float time) {
			this.overTime = Mathf.Abs(overTime);
			this.rotational = Mathf.Abs(rotational);
			this.curvature = Mathf.Abs(curvature);

			this.overTimeSum += Mathf.Abs(overTime);
			this.rotationalSum += Mathf.Abs(rotational);
			this.curvatureSum += Mathf.Abs(curvature);
			this.time = time;
		}
	}
	public class Socket : MonoBehaviour {
		private Scene scene;
		private WorldRedirection script;
		private DateTime startTime;

		private TcpClient client;

		private Razzaque2001Hybrid loggingTechnique;

		private WorldRedirectionData redirectionData;

		private void Start() {
			script = Toolkit.Instance.GetComponent<WorldRedirection>();
			scene = script.scene;
			InvokeRepeating(nameof(StartSendingMessages), 1f, 0.5f);
			loggingTechnique = new();

			redirectionData = new();
		}

		private void StartSendingMessages() {
			if (client == null || !client.Connected) {
				try {
					client = new TcpClient("localhost", 13000);
					startTime = DateTime.Now;
				}
				catch (SocketException) {}
			} else {
				Thread thread = new(() => SendMessage(client, redirectionData));
				thread.Start();
			}
		}

		private void SendMessage(TcpClient client, WorldRedirectionData redirectionData) {
			string json = JsonUtility.ToJson(redirectionData);
			Debug.Log($"Sending: {json}");
			// Translate the passed message into ASCII and store it as a Byte array.
			Byte[] messageBytes = System.Text.Encoding.ASCII.GetBytes(json);

			// Get a client stream for reading and writing.
			NetworkStream stream = client.GetStream();

			try {
				// Send the message to the connected TcpServer.
				stream.Write(messageBytes, 0, messageBytes.Length);
				// stream.Flush();
			} catch (SocketException) {Debug.LogWarning("Socket closed.");}
		}

		private void Update() {
				redirectionData.AddTo((script.redirect) ? Razzaque2001OverTimeRotation.GetRedirection(scene) : 0f,
									  (script.redirect) ? Razzaque2001Rotational.GetRedirection(scene) : 0f,
									  (script.redirect) ? Razzaque2001Curvature.GetRedirection(scene) : 0f,
									  (float)(DateTime.Now - startTime).TotalSeconds);
		}
	}
}
