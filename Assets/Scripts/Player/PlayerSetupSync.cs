using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

class PlayerSetupSync : Photon.MonoBehaviour {
	[HideInInspector] public int playerCount = 0;
	[HideInInspector] public List<bool> playersSpawned = new List<bool> (new bool[]{false, false, false, false});
	public Color[] playerColors = { }; //100, 220, 100 // 255, 100, 100 // 0, 150, 200 // 255, 255, 100

	public Color currentColor;
	public string currentName;

	public void BeginSetup (int startingPixels) {
		playerCount = PhotonNetwork.playerList.Length;
		Debug.Log ("Player count: " + PhotonNetwork.playerList.Length);

		// Detect spawned players
		playersSpawned.Clear();
		playersSpawned.AddRange(new bool[] {false, false, false, false});
		int playerNumber = 1;

		Debug.Log(GameObject.FindGameObjectsWithTag("Player").Length);

		foreach (GameObject go in GameObject.FindGameObjectsWithTag ("Player")) {
			if (!go.GetPhotonView ().isMine) {
				Debug.Log (Regex.Match (go.name, @"\d+$").Value);
				int i = int.Parse (Regex.Match (go.name, @"\d+$").Value);
				playersSpawned [i - 1] = true;
			}
		}

		playersSpawned.ForEach(item => Debug.Log(item));

		playerNumber = playersSpawned.IndexOf (false) + 1;
		playersSpawned[playerNumber - 1] = true;
		//Debug.Log(playersSpawned.IndexOf(false) + 1);

		photonView.RPC("SetupPlayer", PhotonTargets.AllBuffered, playerNumber, startingPixels);
	}

	[PunRPC]
	void SetupPlayer (int playerNumber, int startingPixels) {
		currentName = "Player" + playerNumber;

		currentColor = playerColors[playerNumber - 1];
		currentColor.a = 1.0f;

		GameObject[] spawns = GameObject.FindGameObjectsWithTag("PlayerSpawn");
		Transform currentSpawnPoint = spawns[playerNumber - 1].transform;

		gameObject.name = currentName;
		gameObject.tag = "Player";
		GetComponent<SpriteRenderer>().material.color = currentColor;

		GetComponent<Player>().pixels = startingPixels;

		transform.position = currentSpawnPoint.position;
		transform.rotation = currentSpawnPoint.rotation;

		if (!photonView.isMine) {
			transform.FindChild("HealthBar").gameObject.SetActive(true);
			transform.FindChild("PlayerName").gameObject.SetActive(true);
			transform.FindChild("ClosestSO").gameObject.SetActive(false);
		}
	}
}