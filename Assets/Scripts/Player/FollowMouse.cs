using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FollowMouse : Photon.MonoBehaviour {

	//private GameObject localPlayer;
	private bool canStart = true;

	//private NetMgr netMgr;

	void Start () {
		//netMgr = GameObject.Find("NetworkManager").GetComponent<NetMgr>();

		//StartCoroutine (WaitForPlayer ());
	}

	void Update () {
		if (!photonView.isMine) return;

			if (canStart) {
			Vector3 diff = Camera.main.ScreenToWorldPoint (Input.mousePosition) - transform.position;
			diff.Normalize ();
			float rot_z = Mathf.Atan2 (diff.y, diff.x) * Mathf.Rad2Deg;
			transform.rotation = Quaternion.Euler (0f, 0f, rot_z - 90);
		}
	}

	//IEnumerator WaitForPlayer () {
	//	while (localPlayer == null) {
	//		localPlayer = netMgr.localPlayer;
	//		yield return new WaitForSeconds (0);
	//	}

	//	canStart = true;
	//}
}