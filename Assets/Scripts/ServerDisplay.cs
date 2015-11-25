using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;

public class ServerDisplay : NetworkBehaviour {

	private string activeObjects;

	// Use this for initialization
	void Start () {
		GetComponent<Text> ().text = "";
	}

	// Update is called once per frame
	void Update () {
		if (!isServer)
			return;

		activeObjects = "";
		GetComponent<Text> ().text = "";
		foreach (GameObject go in FindObjectsOfType<GameObject> ()) {
			if (go.transform.parent == null) {
				activeObjects = activeObjects + ", " + go.name;
			}
		}
	
		GetComponent<Text> ().text = activeObjects;
	}
}