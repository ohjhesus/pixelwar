using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FollowSpaceObject : MonoBehaviour {

	private bool canStart;

	void Start () {
		GetComponent<SpriteRenderer> ().enabled = false;
		canStart = false;
		StartCoroutine (SOCheck ());
	}

	void Update () {
		if (canStart) {
			GameObject closestSO = null;
			closestSO = FindClosestSO ();
			if (closestSO == null) {
				GetComponent<SpriteRenderer> ().enabled = false;
			} else {
				GetComponent<SpriteRenderer> ().enabled = true;
				Vector3 diff = closestSO.transform.position - transform.position;
				diff.Normalize ();
				float rot_z = Mathf.Atan2 (diff.y, diff.x) * Mathf.Rad2Deg;
				transform.rotation = Quaternion.Euler (0f, 0f, rot_z - 90);
			}
		}
	}

	GameObject FindClosestSO () {
		GameObject closest = null;
		float distance = Mathf.Infinity;
		Vector3 position = transform.position;
		foreach (GameObject go in GameObject.FindGameObjectsWithTag ("SpaceObject")) {
			Vector3 diff = go.transform.position - position;
			float curDistance = diff.sqrMagnitude;
			if (curDistance < distance) {
				closest = go;
				distance = curDistance;
			}
		}
		return closest;
	}

	IEnumerator SOCheck () {
		while (FindClosestSO () == null)
			yield return new WaitForSeconds (0);

		canStart = true;
	}
}