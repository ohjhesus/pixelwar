using UnityEngine;
using System.Collections;
using System.Threading.Tasks;

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
	private bool canExplode = true;

	private GameObject explosion;
	
	public void StartShot (Shoot shootScript, Vector2 playerVelocity) {
		transform.rotation.eulerAngles.Set(0, 0, shootScript.transform.eulerAngles.z);

		explosion = transform.Find("Explosion").gameObject;

		rib = GetComponent<Rigidbody2D> ();
		rib.velocity = new Vector3 (playerVelocity.x, playerVelocity.y, 0) + (transform.TransformDirection(new Vector3(0, speed, 0)));
		rib.AddTorque(torque);
		startPos = transform.position;
		canStart = true;
	}

	void Update () {
		if (!photonView.isMine) return;

		if (canStart) {
			if (canExplode) {
				if (Vector3.Distance(startPos, transform.position) > travelDistance) {
					canExplode = false;
					Explode();
				}
			}
		}
	}

	public void Explode () {
		photonView.RPC("CreateProjectileExplosion", PhotonTargets.All);

		//photonView.RPC ("MasterDestroyProjectile", PhotonTargets.MasterClient);
		MasterDestroyProjectile();
	}

	[PunRPC]
	void CreateProjectileExplosion () {
		if (explosion != null) {
			explosion.GetComponent<ProjectileExplosion>().enabled = true;
			explosion.transform.SetParent(null);

			GetComponent<SpriteRenderer>().enabled = false;
			rib.velocity = Vector2.zero;
			GetComponent<Collider2D>().enabled = false;

			//canDestroy = true;
		}
	}

	//[PunRPC]
	async void MasterDestroyProjectile () {
		Debug.Log("Destroying projectile");
		await Task.Delay(1000);
		PhotonNetwork.Destroy(gameObject);
	}
}