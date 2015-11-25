using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Projectile : NetworkBehaviour {

	[HideInInspector] [SyncVar] public float speed;
	[HideInInspector] [SyncVar] public float travelDistance;
	[HideInInspector] [SyncVar] public float knockback;
	[HideInInspector] [SyncVar] public int damage;
	
	private Vector3 startPos;
	
	private Rigidbody2D rib;
	
	void Start () {
		rib = GetComponent<Rigidbody2D> ();
		rib.velocity = transform.TransformDirection(new Vector3(0, speed, 0));
		startPos = transform.position;
	}

	void Update () {
		if (!isServer)
			return;

		if (Vector3.Distance (startPos, transform.position) > travelDistance) {
			RpcExplode ();
		}
	}

	[ClientRpc]
	public void RpcExplode () {
		transform.FindChild("Explosion").GetComponent<ProjectileExplosion> ().enabled = true;
		transform.FindChild("Explosion").parent = null;
		
		Destroy (gameObject);
	}
}