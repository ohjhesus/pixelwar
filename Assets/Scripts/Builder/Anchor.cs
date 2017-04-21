using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Anchor : MonoBehaviour {

	private GameObject builderPanel;

	// Use this for initialization
	void Start () {
		builderPanel = GameObject.Find ("GameControllers").GetComponent<Builder> ().builderPanel;
	}

	// Update is called once per frame
	void Update () {
		if (builderPanel.activeInHierarchy) {
			GetComponent<SpriteRenderer> ().enabled = true;
		} else {
			GetComponent<SpriteRenderer> ().enabled = false;
		}
	}
}