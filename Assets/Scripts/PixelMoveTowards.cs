using UnityEngine;
using System.Collections;

public class PixelMoveTowards : MonoBehaviour {

//	int[] controllers;
	private GameObject closestPlayer;
	private float moveDistance;
	private bool canCheckDistance;
	private float rotationAmount;

	// Use this for initialization
	void Start () {
		rotationAmount = Random.Range (-1f, 1f);
		CheckDistance ();
	}
	
	void FixesUpdate () {
//		Debug.Log ("closest player: " + closestPlayer);
		moveDistance = Vector2.Distance (new Vector2 (closestPlayer.transform.position.x, closestPlayer.transform.position.y), new Vector2 (transform.position.x, transform.position.y));
		if (moveDistance <= 4f) {
			transform.position = Vector3.MoveTowards (new Vector3 (transform.position.x, transform.position.y, -1), new Vector3 (closestPlayer.transform.position.x, closestPlayer.transform.position.y, -1), (4 - moveDistance) / 100);
		}

		transform.rotation = Quaternion.Euler (0, 0, transform.rotation.eulerAngles.z + rotationAmount - ((4 - moveDistance) * rotationAmount * 2));

		if (canCheckDistance) {
			canCheckDistance = false;
			CheckDistance ();
		}
	}

	void CheckDistance () {
		int index = 0;
		float closestDistance = Mathf.Infinity;
		float distance;

		foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player")) {
//			controllers[index] = player.GetComponent<NetworkIdentity> ().playerControllerId;
			distance = Vector2.Distance (new Vector2 (player.transform.position.x, player.transform.position.y), new Vector2 (transform.position.x, transform.position.y));
			if (distance < closestDistance) {
				closestDistance = distance;
				closestPlayer = player;
			}
			index++;
		}

		canCheckDistance = true;
	}

	void OnCollisionEnter2D (Collision2D coll) {
		if (coll.gameObject.tag == "Projectile") {
			coll.gameObject.GetComponent<Projectile> ().Explode ();
		}
	}
}