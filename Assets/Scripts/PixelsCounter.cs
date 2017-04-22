using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections;

public class PixelsCounter : MonoBehaviour {

	private GameObject player;
	private NetMgr netMgr;

	void Start () {
		netMgr = GameObject.Find("NetworkManager").GetComponent<NetMgr>();
		player = netMgr.localPlayer;
	}

	void Update () {
		if (player != null) {
			GetComponent<Text> ().text = player.GetComponent<Player> ().pixels + " PIXELS";
            GetComponentInParent<Image>().enabled = true;
        } else {
			player = netMgr.localPlayer;
			GetComponent<Text>().text = "";
			GetComponentInParent<Image>().enabled = false;
		}
	}
}
