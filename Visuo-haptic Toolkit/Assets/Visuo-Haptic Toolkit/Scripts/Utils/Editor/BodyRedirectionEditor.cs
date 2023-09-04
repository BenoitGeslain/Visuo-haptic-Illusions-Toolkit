using UnityEngine;
using UnityEditor;

namespace BG.Redirection {
	[CustomEditor(typeof(BodyRedirection))]
	public class BodyRedirectionEditor : Editor {

		public override void OnInspectorGUI() {
			base.OnInspectorGUI();

			// EditorGUILayout.BeginHorizontal();
			// if(GUILayout.Button("Reset Redirection")) {
			// 	BodyRedirection BRScript = (BodyRedirection) target;
			// 	BRScript.resetRedirection();
			// }
			// // if(GUILayout.Button("Restart Redirection")) {
			// // 	BodyRedirection BRScript = (BodyRedirection) target;
			// // 	BRScript.restartRedirection();
			// // }
			// EditorGUILayout.EndHorizontal();
		}
	}
}