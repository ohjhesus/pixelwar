using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NetMgr : MonoBehaviour {

	private int playerCount = 1;
	private Vector2 startPosition;
	private Quaternion startRotation;
	private Color playerColor;
	public Color[] playerColors;
	public GameObject playerPrefab;

	public void StartGame () {
		GameObject player = Instantiate(playerPrefab, startPosition, startRotation);
		player.name = "player" + playerCount;
		player.GetComponent<Player> ().pixels = 60;

		playerColor = playerColors[playerCount - 1];

		player.GetComponent<Renderer>().material.color = playerColor;
		playerCount++;
	}
}