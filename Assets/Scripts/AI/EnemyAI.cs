using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : Photon.MonoBehaviour {

	public int health;

	[HideInInspector] public List<Shoot> shootScripts;

	private NetOperations netOps;
	private Options options;

	[HideInInspector] public GameObject closestPlayer;

	private Vector2 moveVelocity;
	public float maxVelocity;
	public float minMoveCooldown;
	public float maxMoveCooldown;
	private Rigidbody2D rib;

	// Sync player pixels
	void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
		if (stream.isWriting) {
			stream.SendNext(health);
		} else {
			health = (int)stream.ReceiveNext();
		}
	}

	// Use this for initialization
	void Start () {
		netOps = GameObject.Find ("NetworkManager").GetComponent<NetOperations> ();
		options = GameObject.Find("GameManager").GetComponent<Options>();
		rib = GetComponent<Rigidbody2D> ();

		StartCoroutine (MoveCooldown ());

		foreach (Transform child in transform) {
			if (child.GetComponent<Shoot> ()) {
				shootScripts.Add (child.GetComponent<Shoot> ());
			}
		}
	}

	private IEnumerator MoveCooldown () {
		moveVelocity = new Vector2 (Random.Range (-maxVelocity, maxVelocity), Random.Range (-maxVelocity, maxVelocity));
		yield return new WaitForSeconds (Random.Range (minMoveCooldown, maxMoveCooldown));
		StartCoroutine (MoveCooldown ());
	}

	// Update is called once per frame
	void Update () {
		if (!PhotonNetwork.isMasterClient || netOps == null || options == null)
			return;

		rib.AddForce (moveVelocity);

		closestPlayer = GetClosestPlayer ();

		if (closestPlayer != null) {
			foreach (Shoot shootScript in shootScripts) {
				if (shootScript.canShoot) {
					shootScript.canShoot = false;
					//						shootScript.gameObject.GetComponent<AudioSource>().PlayOneShot(shootScript.gameObject.GetComponent<AudioSource>().clip, shootScript.gameObject.GetComponent<AudioSource>().volume);
					netOps.gameObject.GetPhotonView ().RPC ("RPCPlayOneShot", PhotonTargets.All, photonView.viewID, shootScript.gameObject.name, "Cannon Shots/", shootScript.gameObject.GetComponent<AudioSource> ().clip.name, shootScript.gameObject.GetComponent<AudioSource> ().volume);
					if (options.chromaticAberration) {
						StartCoroutine (shootScript.FadeAberration ());
					}
					Fire (shootScripts.IndexOf (shootScript));
					StartCoroutine (shootScript.Cooldown ());
				}
			}
		}
	}

	public void Fire (int shootScriptIndex) {
//		if (!photonView.isMine) return; // exit Fire function if player isn't ours (multiplayer)

		Shoot shootScript = shootScripts[shootScriptIndex]; // get Shoot script of cannon that shot

		GameObject shot = PhotonNetwork.InstantiateSceneObject(shootScript.projectile.name, shootScript.transform.position + (shootScript.transform.up / 3), shootScript.transform.rotation, 0, null); // create projectile object
		Physics2D.IgnoreCollision(shot.GetComponent<Collider2D>(), GetComponent<Collider2D>()); // ignore collision between projectile and our own ship (avoid shooting ourself)

		shot.transform.rotation = shootScript.transform.rotation; // make projectile start facing correct direction
		shot.transform.rotation = Quaternion.Euler(0, 0, shootScript.transform.rotation.eulerAngles.z + Random.Range(-shootScript.aimCone, shootScript.aimCone));
		shot.GetComponent<Projectile>().speed = shootScript.projectileSpeedMultiplier * 13; // speed = player speed + projectile speed
		shot.GetComponent<Projectile>().torque = shootScript.projectileTorque; // allow projectile to rotate
		shot.GetComponent<Projectile>().knockback = shootScript.projectileKnockback / 10; // set knockback applied to objects collided with
		shot.GetComponent<Projectile>().damage = shootScript.projectileDamage; // set damage
		shot.GetComponent<Projectile>().travelDistance = shootScript.projectileTravelDistance; // set how far projectile can travel before it is destroyed

		shot.GetComponent<Projectile>().StartShot(shootScript, new Vector2(0,0), null); // start projectile movement

		photonView.RPC("SetupShot", PhotonTargets.AllBufferedViaServer, shot.GetPhotonView().viewID, shootScriptIndex); // setup shot for all players
	}

	[PunRPC]
	void SetupShot (int shotViewID, int shootScriptIndex) {
		GameObject shot = PhotonView.Find(shotViewID).gameObject; // find projectile by its multiplayer object ID
		Shoot shootScript = shootScripts[shootScriptIndex]; // get Shoot script of cannon that shot

		//Debug.Log(shootScript.gameObject.name);

		shot.name = name + shootScript.projectile.name; // set projectile name
		shot.tag = "Projectile"; // set projectile tag

		Physics2D.IgnoreCollision(shot.GetComponent<Collider2D>(), GetComponent<Collider2D>()); // ignore collision between projectile and our own ship

		GameObject[] otherProjectiles = GameObject.FindGameObjectsWithTag("Projectile");
		foreach (GameObject go in otherProjectiles) {
			Physics2D.IgnoreCollision(shot.GetComponent<Collider2D>(), go.GetComponent<Collider2D>()); // ignore collision with other projectiles
		}
	}

	GameObject GetClosestPlayer () {
		GameObject closest = null;

		float distance = 13f;
		Vector2 position = transform.position;
		foreach (GameObject go in GameObject.FindGameObjectsWithTag("Player")) {
			float diff = Vector2.Distance (go.transform.position, position);
			//Debug.Log (go.name + diff);
			if (diff < distance) {
				closest = go;
				distance = diff;
			}
		}

		return closest;
	}
}