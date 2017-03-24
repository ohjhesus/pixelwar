using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FollowMouse : MonoBehaviour {

	private GameObject localPlayer;
	private bool canStart;

	void Start () {
		StartCoroutine (WaitForPlayer ());
	}

	void Update () {
		if (canStart) {
			Vector3 diff = Camera.main.ScreenToWorldPoint (Input.mousePosition) - transform.position;
			diff.Normalize ();
			float rot_z = Mathf.Atan2 (diff.y, diff.x) * Mathf.Rad2Deg;
			transform.rotation = Quaternion.Euler (0f, 0f, rot_z - 90);
		}
	}

	IEnumerator WaitForPlayer () {
		while (localPlayer == null) {
			localPlayer = GameObject.Find("player1");
			yield return new WaitForSeconds (0);
		}

		canStart = true;
	}
}