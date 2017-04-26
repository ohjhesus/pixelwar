using UnityEngine;
using System.Collections;

public class Projectile : Photon.MonoBehaviour {

	[HideInInspector] public float speed;
	[HideInInspector] public float torque;
	[HideInInspector] public float travelDistance;
	[HideInInspector] public float knockback;
	[HideInInspector] public int damage;
	public GameObject[] otherProjectiles;
	
	private Vector3 startPos;
	
	private Rigidbody2D rib;

	public bool canStart = false;
	
	public void StartShot (Shoot shootScript) {
		transform.rotation.eulerAngles.Set(0, 0, shootScript.transform.eulerAngles.z);

		rib = GetComponent<Rigidbody2D> ();
		rib.velocity = transform.TransformDirection(new Vector3(0, speed, 0));
		rib.AddTorque(torque);
		startPos = transform.position;
		canStart = true;
	}

	void Update () {
		if (canStart) {
			if (Vector3.Distance(startPos, transform.position) > travelDistance) {
				Explode();
			}
		}
	}

	public void Explode () {
		photonView.RPC("CreateExplosion", PhotonTargets.AllViaServer);
	}

	[PunRPC]
	void CreateExplosion () {
		transform.FindChild("Explosion").GetComponent<ProjectileExplosion>().enabled = true;
		transform.FindChild("Explosion").parent = null;

		if (PhotonNetwork.isMasterClient) {
			PhotonNetwork.Destroy(gameObject);
		}
	}
}