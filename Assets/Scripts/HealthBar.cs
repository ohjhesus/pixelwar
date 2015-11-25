using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections;

public class HealthBar : NetworkBehaviour {

	private float health;
	private Image img;

	void Start () {
		img = GameObject.Find ("HealthBarDisplay").GetComponent<Image> ();
	}

	// Update is called once per frame
	void Update () {
		if (!isLocalPlayer)
			return;

		health = Mathf.Clamp (1f - GetComponent<Player> ().pixels / 200f, 0.5f, 1f);
		if (health != 1f) {
			img.enabled = true;
			img.material.SetFloat ("_Cutoff", health);
		} else {
			img.enabled = false;
		}
	}
}