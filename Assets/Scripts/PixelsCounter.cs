using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections;

public class PixelsCounter : MonoBehaviour {

	private GameObject player;

	void Start () {
		player = GameObject.Find("player1");
	}

	void Update () {
		if (player != null) {
			GetComponent<Text> ().text = player.GetComponent<Player> ().pixels + " PIXELS";
            GetComponentInParent<Image>().enabled = true;
        } else {
			player = GameObject.Find("player1");
			GetComponent<Text>().text = "";
			GetComponentInParent<Image>().enabled = false;
		}
	}
}
