using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class NetMgr : NetworkManager {

	private int playerCount;
	private Vector2 startPosition;
	private Quaternion startRotation;
	private Color playerColor;
	public Color[] playerColors;

	public override void OnStartServer () {
		Debug.Log ("Server started");

		playerCount = 1;
	}

	public override void OnServerAddPlayer (NetworkConnection conn, short playerControllerID) {
		Debug.Log ("Player joined server (" + playerCount + ")");

		switch (playerCount) {
			case 1:
				startPosition = new Vector2 (-20, -20);
				startRotation = Quaternion.Euler (new Vector3 (0, 0, -45));
				playerColor = playerColors[0];
				break;
			case 2:
				startPosition = new Vector2 (20, 20);
				startRotation = Quaternion.Euler (new Vector3 (0, 0, 135));
				playerColor = playerColors[1];
				break;
			case 3:
				startPosition = new Vector2 (-20, 20);
				startRotation = Quaternion.Euler (new Vector3 (0, 0, -135));
				playerColor = playerColors[2];
				break;
			case 4:
				startPosition = new Vector2 (20, -20);
				startRotation = Quaternion.Euler (new Vector3 (0, 0, 45));
				playerColor = playerColors[3];
				break;
		}

		GameObject player = (GameObject)Instantiate (playerPrefab, startPosition, startRotation);
		player.GetComponent<NameSync> ().myName = "player" + playerCount;
		player.name = "player" + playerCount;
		player.GetComponent<Player> ().pixels = 120;
		player.GetComponent<ColorSync> ().myColor = playerColor;
		player.GetComponent<Renderer>().material.color = playerColor;
		NetworkServer.AddPlayerForConnection (conn, player, playerControllerID);
		playerCount++;
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
