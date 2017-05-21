using UnityEngine;
using System.Collections;

public class PlayerCollisions : Photon.MonoBehaviour {

	void OnCollisionEnter2D (Collision2D coll) {
		if (coll.gameObject.tag == "Projectile") {
			coll.gameObject.GetComponent<Collider2D> ().enabled = false; // disable projectile collider to avoid multiple collisions

			Debug.Log(name + " hit!");
			GetComponent<Rigidbody2D>().AddRelativeForce(Vector2.up * coll.gameObject.GetComponent<Projectile>().knockback * 100f); // apply knockback force to player hit

			GetComponent<Player> ().AffectHealth (coll.gameObject.GetComponent<Projectile> ().damage); // apply projectile's damage to player
			
			coll.gameObject.GetComponent<Projectile> ().Explode (); // create projectile explosion and destroy projectile
		}
	}
}