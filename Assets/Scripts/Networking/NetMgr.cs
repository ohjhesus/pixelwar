using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class NetMgr : Photon.MonoBehaviour {

	public string gameVersion = "1.0.0";
	public bool dontDestroyOnLoad = true;
	private int playerCount = 0;
	private Color playerColor;
	public Color[] playerColors;
	public GameObject localPlayer;

	private bool hasJoinedLobby = false;

	private void Start() {
		Connect();

		if (dontDestroyOnLoad) {
			DontDestroyOnLoad(gameObject);
		}
	}

	private void OnEnable () {
		SceneManager.sceneLoaded += OnLevelLoaded;
		PhotonNetwork.OnEventCall += OnEvent;
	}

	private void OnDisable() {
		SceneManager.sceneLoaded -= OnLevelLoaded;
		PhotonNetwork.OnEventCall -= OnEvent;
	}

	// PLAYER CONNECTIONS

	private void Connect () {
		PhotonNetwork.ConnectUsingSettings(gameVersion);
	}

	private void OnPhotonPlayerConnected (PhotonPlayer connected) {
		Debug.Log("NET: Player connected");
	}

	private void OnPhotonPlayerDisconnected() {
		Debug.Log("NET: Player disconnected");
	}
	
	// CREATE ROOM

	private void CreateRoom () {
		PhotonNetwork.CreateRoom(null);
	}

	// JOIN ROOM

	private void OnJoinedRoom() {
		Debug.Log("NET: Joined room: " + PhotonNetwork.room.Name);
		PhotonNetwork.LoadLevel(1);	
	}

	private void OnLevelLoaded (Scene scene, LoadSceneMode mode) {
		if (scene.buildIndex == 1) {
			SpawnPlayer();
		}
	}

	// JOIN RANDOM ROOM

	public void JoinRandomRoom () {
		if (hasJoinedLobby) {
			Debug.Log("NET: Joining random room");
			PhotonNetwork.JoinRandomRoom();
		} else {
			Debug.Log("NET: Failed joining random room; player has not joined lobby");
		}
	}

	public void OnPhotonRandomJoinFailed() {
		Debug.Log("NET: Failed joining random room");
		CreateRoom();
	}

	// JOIN LOBBY

	private void OnJoinedLobby () {
		Debug.Log("NET: Joined lobby");
		hasJoinedLobby = true;
	}

	// PLAYER SPAWNING / RESPAWNING

	private void SpawnPlayer () {
		GameObject[] spawns = GameObject.FindGameObjectsWithTag("PlayerSpawn");
		Transform currentSpawnPoint = spawns[playerCount].transform;

		GameObject player = PhotonNetwork.Instantiate("Player", currentSpawnPoint.position, currentSpawnPoint.rotation, 0);
		if (player.GetComponent<PhotonView>().isMine) {
			localPlayer = player;
		}

		playerCount = 0;
		foreach (GameObject go in GameObject.FindGameObjectsWithTag("Player")) {
			playerCount++;
		}

		player.name = "player" + playerCount;
		player.GetComponent<Player>().pixels = 60;

		playerColor = playerColors[playerCount - 1];

		player.GetComponent<Renderer>().material.color = playerColor;
	}

	private void Respawn() {
		PhotonNetwork.DestroyPlayerObjects(PhotonNetwork.player);
		SpawnPlayer();
	}

	// RPCs

	private void OnEvent (byte eventCode, object content, int senderid) {
		if (eventCode == 0) { // increment playerCount
			playerCount++;
		}
	}
}