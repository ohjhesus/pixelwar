using UnityEngine;
// using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections;

public class HealthBar : MonoBehaviour {

	private GameObject player;
	private float health;
	private Image img;

	void Start () {
		player = GameObject.Find("player1");
		img = GetComponent<Image> ();
	}

	// Update is called once per frame
	void Update () {
//		transform.position = Camera.main.ScreenToWorldPoint (new Vector3 (28, 35, 0));

		// Debug.Log(img.material.GetFloat("_Cutoff"));

		if (player == null) {
			player = GameObject.Find("player1");
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
