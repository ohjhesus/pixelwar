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

	private GameObject explosion;
	
	public void StartShot (Shoot shootScript) {
		transform.rotation.eulerAngles.Set(0, 0, shootScript.transform.eulerAngles.z);

		explosion = transform.FindChild("Explosion").gameObject;

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

		StartCoroutine(DestroyProjectile());
	}

	[PunRPC]
	void CreateExplosion () {
		if (explosion != null) {
			explosion.GetComponent<ProjectileExplosion>().enabled = true;
			explosion.transform.SetParent(null);

			GetComponent<SpriteRenderer>().enabled = false;
			rib.velocity = Vector2.zero;
			GetComponent<Collider2D>().enabled = false;

			//canDestroy = true;
		}
	}

	IEnumerator DestroyProjectile () {
		if (PhotonNetwork.isMasterClient) {
			yield return new WaitForSeconds(1f);
			PhotonNetwork.Destroy(gameObject);
		}
	}
}