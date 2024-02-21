using System;
using System.Linq;
using System.Net.Sockets;

using UnityEngine;

using VHToolkit.Redirection;
using VHToolkit.Redirection.WorldRedirection;

namespace VHToolkit.Logging {

	[Serializable]
	struct WorldRedirectionData {
		[SerializeField] float overTime, rotational, curvature, hybrid, total;
		[SerializeField] float time;

		public void Add(float overTime, float rotational, float curvature, float hybrid, float total, float time) {
			this.overTime += overTime;
			this.rotational += rotational;
			this.curvature += curvature;
			this.hybrid += hybrid;
			this.total += total;
			this.time = time;
		}
	}
	public class Socket : MonoBehaviour {
		private Scene scene;
		private DateTime startTime;

		private TcpClient client;

		private Razzaque2001Hybrid loggingTechnique;

		private WorldRedirectionData redirectionData;

		private void Start() {
			scene = Toolkit.Instance.GetComponent<WorldRedirection>().scene;
			startTime = DateTime.Now;
			InvokeRepeating(nameof(SendMessage), 1f, 0.5f);
			loggingTechnique = new();
		}

		private void SendMessage() {
			if (client == null) {
				// Prefer a using declaration to ensure the instance is Disposed later.
				client = new TcpClient("localhost", 13000);
			} else {
				string json = JsonUtility.ToJson(redirectionData);
				Debug.Log($"Sending: {json}");
				// Translate the passed message into ASCII and store it as a Byte array.
				Byte[] data = System.Text.Encoding.ASCII.GetBytes(json);

				// Get a client stream for reading and writing.
				NetworkStream stream = client.GetStream();

				// Send the message to the connected TcpServer.
				stream.Write(data, 0, data.Length);
				stream.Flush();
				redirectionData = new();
			}
		}

		private void Update() {
			redirectionData.Add(Razzaque2001OverTimeRotation.GetRedirection(scene),
								Razzaque2001Rotational.GetRedirection(scene),
								Razzaque2001Curvature.GetRedirection(scene),
								loggingTechnique.GetRedirection(scene),
								scene.HeadToHeadRedirection.eulerAngles.y,
								(float)(DateTime.Now - startTime).TotalSeconds);
		}
	}
}
