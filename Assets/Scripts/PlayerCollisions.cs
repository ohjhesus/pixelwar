using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class PlayerCollisions : NetworkBehaviour {
	

	void OnCollisionEnter2D (Collision2D coll) {
		if (coll.gameObject.tag == "Projectile") {
			coll.gameObject.GetComponent<Collider2D> ().enabled = false;
			RpcAddKnockback (coll.gameObject);
			GetComponent<Player> ().AffectHealth(coll.gameObject.GetComponent<Projectile> ().damage);
			
			coll.gameObject.GetComponent<Projectile> ().RpcExplode();
		}
	}

	[ClientRpc]
	public void RpcAddKnockback (GameObject go) {
		Debug.Log (name + Vector2.down * go.GetComponent<Projectile> ().knockback);
		GetComponent<Rigidbody2D> ().AddRelativeForce (Vector2.down * go.GetComponent<Projectile> ().knockback);
	}
}