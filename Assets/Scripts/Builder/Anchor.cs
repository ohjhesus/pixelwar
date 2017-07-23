using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Anchor : MonoBehaviour {

	private GameObject builderPanel;
	public bool showAnchor;

	// Use this for initialization
	void Start () {
		
	}

	// Update is called once per frame
	void Update () {
		if (builderPanel == null) builderPanel = GameObject.Find ("GameManager").GetComponent<Builder> ().builderPanel;

		if (builderPanel.activeInHierarchy) {
			if (showAnchor) {
				GetComponent<SpriteRenderer> ().enabled = true;
			}
		} else {
			GetComponent<SpriteRenderer> ().enabled = false;
		}
	}
}