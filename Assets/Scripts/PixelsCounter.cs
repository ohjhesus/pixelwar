using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections;

public class PixelsCounter : MonoBehaviour {

	private Player localPlayer;
	
	void Start () {

	}

	void UpdatePlayer () {
		foreach (GameObject go in GameObject.FindGameObjectsWithTag("Player")) {
			if (go.GetComponent<NetworkIdentity> ().isLocalPlayer) {
				localPlayer = go.GetComponent <Player> ();
			}
		}
	}

	void Update () {
		UpdatePlayer ();
		if (localPlayer != null) {
			GetComponent<Text> ().text = localPlayer.pixels + " PIXELS";
		} else {
			GetComponent<Text> ().text = "";
		}
	}
}