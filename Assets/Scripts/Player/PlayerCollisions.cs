using UnityEngine;
using System.Collections;

public class PlayerCollisions : MonoBehaviour {

	void OnCollisionEnter2D (Collision2D coll) {
		if (coll.gameObject.tag == "Projectile") {
			coll.gameObject.GetComponent<Collider2D> ().enabled = false;

			Debug.Log(name + Vector2.down * coll.gameObject.GetComponent<Projectile>().knockback);
			GetComponent<Rigidbody2D>().AddRelativeForce(Vector2.down * coll.gameObject.GetComponent<Projectile>().knockback);

			GetComponent<Player> ().AffectHealth (coll.gameObject.GetComponent<Projectile> ().damage);
			
			coll.gameObject.GetComponent<Projectile> ().Explode ();
		} else if (coll.gameObject.name.Contains("1Pixel")) {
			GetComponent<Player> ().AffectHealth (1);
			PhotonNetwork.Destroy (coll.gameObject);
		} else if (coll.gameObject.name.Contains("5Pixel")) {
			GetComponent<Player> ().AffectHealth (5);
			PhotonNetwork.Destroy (coll.gameObject);
		} else if (coll.gameObject.name.Contains("10Pixel")) {
			GetComponent<Player> ().AffectHealth (10);
			PhotonNetwork.Destroy (coll.gameObject);
		}
	}
}