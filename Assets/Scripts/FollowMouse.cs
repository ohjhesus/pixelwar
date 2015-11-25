using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class FollowMouse : NetworkBehaviour {

	public List<GameObject> cannons = new List<GameObject>();

	private bool canStart;

	void Start () {
		canStart = false;
		StartCoroutine (CannonCheck ());
	}

	void Update () {
		if (isLocalPlayer && canStart) {
			foreach (GameObject cannon in cannons) {
				Vector3 diff = Camera.main.ScreenToWorldPoint (Input.mousePosition) - cannon.transform.position;
				diff.Normalize ();
				float rot_z = Mathf.Atan2 (diff.y, diff.x) * Mathf.Rad2Deg;
				cannon.transform.rotation = Quaternion.Euler (0f, 0f, rot_z - 90);
			}
		}
	}

	IEnumerator CannonCheck () {
		while (cannons.Count == 0)
			yield return new WaitForSeconds (0);

		canStart = true;
	}
}