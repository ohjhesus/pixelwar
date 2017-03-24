using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour {

	[HideInInspector] public float speed;
	[HideInInspector] public float torque;
	[HideInInspector] public float travelDistance;
	[HideInInspector] public float knockback;
	[HideInInspector] public int damage;
	public GameObject[] otherProjectiles;
	
	private Vector3 startPos;
	
	private Rigidbody2D rib;
	
	void Start () {
		rib = GetComponent<Rigidbody2D> ();
		rib.velocity = transform.TransformDirection(new Vector3(0, speed, 0));
		rib.AddTorque(torque);
		startPos = transform.position;
	}

	void Update () {
		if (Vector3.Distance (startPos, transform.position) > travelDistance) {
			Explode ();
		}
	}

	public void Explode () {
		transform.FindChild("Explosion").GetComponent<ProjectileExplosion> ().enabled = true;
		transform.FindChild("Explosion").parent = null;
		
		Destroy (gameObject);
	}
}