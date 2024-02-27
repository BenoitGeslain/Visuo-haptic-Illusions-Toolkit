using System;
using System.Net.Sockets;
using System.Threading;

using UnityEngine;

using VHToolkit.Redirection;
using VHToolkit.Redirection.WorldRedirection;

namespace VHToolkit.Logging {

	[Serializable]
	struct WorldRedirectionData {
		[SerializeField] public float overTime, rotational, curvature, hybrid, total;
		[SerializeField] float time;

		public void AddTo(float overTime, float rotational, float curvature, float hybrid, float total, float time) {
			this.overTime += Mathf.Abs(overTime);
			this.rotational += Mathf.Abs(rotational);
			this.curvature += Mathf.Abs(curvature);
			this.hybrid += Mathf.Abs(hybrid);
			this.total = total;
			this.time = time;
		}
	}
	public class Socket : MonoBehaviour {
		private Scene scene;
		private DateTime startTime;

		private TcpClient client;

		private Razzaque2001Hybrid loggingTechnique;

		private WorldRedirectionData redirectionData = new();

		private void Start() {
			scene = Toolkit.Instance.GetComponent<WorldRedirection>().scene;
			InvokeRepeating(nameof(StartSendingMessages), 1f, 0.5f);
			loggingTechnique = new();
		}

		private void StartSendingMessages() {
			if (client == null || !client.Connected) {
				try {
					client = new TcpClient("localhost", 13000) {
						ReceiveTimeout = 500
					};
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
			Debug.Log(json);
			Debug.Log($"Sending: {json}");
			// Translate the passed message into ASCII and store it as a Byte array.
			Byte[] messageBytes = System.Text.Encoding.ASCII.GetBytes(json);

			// Get a client stream for reading and writing.
			NetworkStream stream = client.GetStream();

			try {
				// Send the message to the connected TcpServer.
				stream.Write(messageBytes, 0, messageBytes.Length);
				stream.Flush();
			} catch (SocketException) {Debug.LogWarning("Socket closed.");}
		}

		private void Update() {
				redirectionData.AddTo(Razzaque2001OverTimeRotation.GetRedirection(scene),
									Razzaque2001Rotational.GetRedirection(scene),
									Razzaque2001Curvature.GetRedirection(scene),
									loggingTechnique.GetRedirection(scene),
									(scene.HeadToHeadRedirection.eulerAngles.y > 180f) ? 360f - scene.HeadToHeadRedirection.eulerAngles.y : scene.HeadToHeadRedirection.eulerAngles.y,
									(float)(DateTime.Now - startTime).TotalSeconds);
		}
	}
}
