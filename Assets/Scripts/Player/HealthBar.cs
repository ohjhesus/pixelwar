using UnityEngine;
// using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections;

public class HealthBar : MonoBehaviour {

	private GameObject player;
	private float health;
	private Image img;
	private Material mat;

	private NetMgr netMgr;

	void Start () {
		netMgr = GameObject.Find("NetworkManager").GetComponent<NetMgr>();

		player = netMgr.localPlayer;
		img = GetComponent<Image> ();
		mat = (Material)Instantiate (img.material);
		img.material = mat;
	}

	// Update is called once per frame
	void Update () {
//		transform.position = Camera.main.ScreenToWorldPoint (new Vector3 (28, 35, 0));

		// Debug.Log(img.material.GetFloat("_Cutoff"));

		if (player == null) {
			player = netMgr.localPlayer;
			return;
		}

		health = Mathf.Clamp (1f - player.GetComponent<Player> ().pixels / 200f, 0.5f, 1f);
		if (health != 1f) {
			img.enabled = true;
			img.material.SetFloat ("_Cutoff", health);
		} else {
			img.enabled = false;
		}
		//img.material.SetFloat("_Cutoff", 1f);
	}
}
