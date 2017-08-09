using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FollowMouse : Photon.MonoBehaviour {

	public bool isPlayerOwned = true;

	void Update () {
		if (!photonView.isMine) return;

		if (isPlayerOwned) {
			Vector3 diff = Camera.main.ScreenToWorldPoint (Input.mousePosition) - transform.position;
			diff.Normalize ();
			float rot_z = Mathf.Atan2 (diff.y, diff.x) * Mathf.Rad2Deg;
			transform.rotation = Quaternion.Euler (0f, 0f, rot_z - 90);
		} else {
			if (transform.parent.GetComponent<EnemyAI> ().closestPlayer != null) {
				Vector3 diff = transform.parent.GetComponent<EnemyAI> ().closestPlayer.transform.position - transform.position;
				diff.Normalize ();
				float rot_z = Mathf.Atan2 (diff.y, diff.x) * Mathf.Rad2Deg;
				transform.rotation = Quaternion.Euler (0f, 0f, rot_z - 90);
			}
		}
	}
}