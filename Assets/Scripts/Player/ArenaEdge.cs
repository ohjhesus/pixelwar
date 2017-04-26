using UnityEngine;
using System.Collections;

public class ArenaEdge : MonoBehaviour {

	void OnTriggerEnter2D (Collider2D trigen) {
		if (trigen.gameObject.tag == "Player") {
			trigen.GetComponent<Player> ().outOfArena = false;
		}
	}

	void OnTriggerExit2D (Collider2D trigex) {
		if (trigex.gameObject.tag == "Player") {
			trigex.GetComponent<Player> ().outOfArena = true;
		}
	}
}
