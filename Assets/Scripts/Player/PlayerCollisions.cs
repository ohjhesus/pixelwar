using UnityEngine;
using System.Collections;

public class PlayerCollisions : Photon.MonoBehaviour {

	void OnCollisionEnter2D (Collision2D coll) {
		if (coll.gameObject.tag == "Projectile") {
			coll.gameObject.GetComponent<Collider2D> ().enabled = false;

			Debug.Log(name + " hit!");
			GetComponent<Rigidbody2D>().AddRelativeForce(Vector2.up * coll.gameObject.GetComponent<Projectile>().knockback * 100f);

			GetComponent<Player> ().AffectHealth (coll.gameObject.GetComponent<Projectile> ().damage);
			
			coll.gameObject.GetComponent<Projectile> ().Explode ();
		}
	}
}