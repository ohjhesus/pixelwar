using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FollowPlayer : MonoBehaviour {

	private bool canStart;
	public bool isEnabled = false;

	void Start () {
		GetComponent<SpriteRenderer> ().enabled = false;
		canStart = false;
		StartCoroutine (PlayerCheck ());
	}

	void Update () {
		if (canStart) {
			if (isEnabled) {
				GameObject closestEnemy = null;
				closestEnemy = FindClosestEnemy ();
				if (closestEnemy == null) {
					GetComponent<SpriteRenderer> ().enabled = false;
				} else {
					GetComponent<SpriteRenderer> ().enabled = true;
					Vector3 diff = closestEnemy.transform.position - transform.position;
					diff.Normalize ();
					float rot_z = Mathf.Atan2 (diff.y, diff.x) * Mathf.Rad2Deg;
					transform.rotation = Quaternion.Euler (0f, 0f, rot_z - 90);
				}
			}
		}
	}

	GameObject FindClosestEnemy () {
		GameObject closest = null;
		float distance = Mathf.Infinity;
		Vector3 position = transform.position;
		foreach (GameObject go in GameObject.FindGameObjectsWithTag ("Player")) {
			if (go != transform.parent.gameObject) {
				Vector3 diff = go.transform.position - position;
				float curDistance = diff.sqrMagnitude;
				if (curDistance < distance) {
					closest = go;
					distance = curDistance;
				}
			}
		}
		return closest;
	}

	IEnumerator PlayerCheck () {
		while (FindClosestEnemy () == null)
			yield return new WaitForSeconds (0);

		canStart = true;
	}
}
