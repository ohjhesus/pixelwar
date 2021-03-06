using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class NetMgr : Photon.MonoBehaviour {

	public string gameVersion = "1.0.0";
	public string appID = "89eb7daf-04cd-4f1a-8dd1-c141d4b0811d";

	public string serverIP;
	public int serverPort = 5055;

	public bool dontDestroyOnLoad = true;
	public int startingPixels = 60;
	[HideInInspector] public GameObject localPlayer;

	private bool hasJoinedLobby = false;

	public GameObject loadingPanel;

	private void Start() {
		loadingPanel.SetActive(false);

		PhotonNetwork.autoJoinLobby = true;
		PhotonNetwork.MaxResendsBeforeDisconnect = 15;

		//PhotonNetwork.networkingPeer.DebugOut = ExitGames.Client.Photon.DebugLevel.ALL;
		//PhotonNetwork.logLevel = PhotonLogLevel.Full;

		if (dontDestroyOnLoad) {
			DontDestroyOnLoad(gameObject);
		}
	}

	private void OnEnable () {
		SceneManager.sceneLoaded += OnLevelLoaded;
	}

	private void OnDisable() {
		SceneManager.sceneLoaded -= OnLevelLoaded;
	}

	// PLAYER CONNECTIONS

	private void ConnectOnline () {
		Debug.Log ("NET: Joining Online");

		//PhotonNetwork.ConnectToBestCloudServer (gameVersion);
		PhotonNetwork.ConnectUsingSettings(gameVersion);
		PhotonNetwork.JoinLobby();
	}

	private void ConnectLAN (string address, int port) {
		Debug.Log ("NET: Joining LAN - " + address + ":" + port);

		PhotonNetwork.ConnectToMaster (address, port, appID, gameVersion);
	}

	public void Disconnect () {
		PhotonNetwork.Disconnect ();
		PhotonNetwork.LoadLevel (0);
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
			StartCoroutine (SpawnPlayer());
		}
	}

	// JOIN RANDOM ROOM

	private void JoinRandomRoom () {
		if (hasJoinedLobby) {
			Debug.Log("NET: Joining random room");
			PhotonNetwork.JoinRandomRoom();
		} else {
			Debug.Log("NET: Failed joining random room; player has not joined lobby");
		}
	}

	private void OnPhotonRandomJoinFailed() {
		Debug.Log("NET: Failed joining random room");
		CreateRoom();
	}

	// BUTTON HANDLERS

	public void StartOnlineMultiplayer () {
		loadingPanel.SetActive(true);

		ConnectOnline ();
	}

	public void StartLANMultiplayer (string address, int port) {
		Debug.Log("start lan");

		loadingPanel.SetActive(true);

		serverIP = address;
		serverPort = port;
		ConnectLAN (address, port);
	}

	public void StartOffline () {
		loadingPanel.SetActive(true);

		PhotonNetwork.offlineMode = true;

		OnJoinedLobby ();
	}

	// JOIN LOBBY

	private void OnJoinedLobby () {
		Debug.Log("NET: Joined lobby");
		hasJoinedLobby = true;
		JoinRandomRoom ();
	}

	// PLAYER SPAWNING / RESPAWNING

	private IEnumerator SpawnPlayer () {
		Debug.Log("Spawning player");
		GameObject player = PhotonNetwork.Instantiate("Player", Vector3.zero, Quaternion.identity, 0);

		while (player == null)
			yield return new WaitForSeconds(0);

		player.tag = "Player";
		localPlayer = player;

		yield return new WaitForSeconds(1f);
		player.GetComponent<PlayerSetupSync>().BeginSetup(startingPixels);
	}

	public void Respawn() {
		Debug.Log("respawn 2");
		PhotonNetwork.DestroyPlayerObjects(PhotonNetwork.player);
		StartCoroutine (SpawnPlayer());
	}
}