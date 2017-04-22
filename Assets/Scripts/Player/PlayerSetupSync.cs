using UnityEngine;
using System.Collections;

class PlayerSetupSync : Photon.MonoBehaviour {
	[HideInInspector] public int playerCount = 0;
	public Color currentColor;
	public string currentName; 
	public Color[] playerColors;

	public void BeginSetup (int startingPixels) {
		playerCount = PhotonNetwork.playerList.Length;

		photonView.RPC("SetupPlayer", PhotonTargets.AllBufferedViaServer, playerCount, startingPixels);
	}

	[PunRPC]
	void SetupPlayer (int numberOfPlayers, int startingPixels) {
		currentName = "Player" + numberOfPlayers;
		currentColor = playerColors[numberOfPlayers - 1];
		GameObject[] spawns = GameObject.FindGameObjectsWithTag("PlayerSpawn");
		Transform currentSpawnPoint = spawns[numberOfPlayers].transform;

		gameObject.name = currentName;
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