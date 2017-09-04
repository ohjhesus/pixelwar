using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : Photon.MonoBehaviour {

	public float baseSpeed;
	private float totalSpeed;
	public float baseTurnSpeed;
	private float totalTurnSpeed;

	private Rigidbody2D rib;
	private bool movingForwards;
	public GameObject flame;
	public GameObject flameTrail;
	private float currentLerpTime;
	public float lerpTime;

	private bool canFadeIn;
	private bool canFadeOut;

	public int pixels;

	[HideInInspector] public List<Shoot> shootScripts;
	private int attachmentCount;

	private Options options;
	private GameObject pauseMenu;
	private GameObject builderMenu;
	private GameObject deathCanvas;

	[HideInInspector] public bool outOfArena;

	public List<GameObject> attachments;

	private NetOperations netOps;

	// Sync player pixels
	void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
		if (stream.isWriting) {
			stream.SendNext(pixels);
		} else {
			pixels = (int)stream.ReceiveNext();
		}
	}

	void Start () {
		if (photonView.isMine) {
			transform.Find("HealthBar").gameObject.SetActive(false);
			transform.Find("PlayerName").gameObject.SetActive(false);

			pauseMenu = GameObject.Find("GameManager").GetComponent<Pause>().pausePanel;
			builderMenu = GameObject.Find("GameManager").GetComponent<Builder>().builderPanel;
			options = GameObject.Find("GameManager").GetComponent<Options>();
			netOps = GameObject.Find ("NetworkManager").GetComponent<NetOperations> ();
			deathCanvas = GameObject.Find("DeathCanvas");
			deathCanvas.SetActive(false);
		}

		rib = GetComponent<Rigidbody2D> ();
		shootScripts = new List<Shoot> ();
		canFadeIn = true;
		attachmentCount = 0;

		totalSpeed = baseSpeed;
		totalTurnSpeed = baseTurnSpeed;
	}

	void Update () {
		if (canFadeIn && movingForwards) {
			photonView.RPC ("FadeIn", PhotonTargets.All);
			canFadeIn = false;
			canFadeOut = true;
		}
		if (canFadeOut && !movingForwards) {
			photonView.RPC ("FadeOut", PhotonTargets.All);
			canFadeOut = false;
			canFadeIn = true;
		}

		if (!photonView.isMine) return;

		if (Input.GetMouseButton(0)) {
			if (!pauseMenu.activeInHierarchy && !builderMenu.activeInHierarchy) {

				foreach (Shoot shootScript in shootScripts) {
					if (shootScript.canShoot) {
						shootScript.canShoot = false;
//						shootScript.gameObject.GetComponent<AudioSource>().PlayOneShot(shootScript.gameObject.GetComponent<AudioSource>().clip, shootScript.gameObject.GetComponent<AudioSource>().volume);
						netOps.gameObject.GetPhotonView().RPC("RPCPlayOneShot", PhotonTargets.All, photonView.viewID, shootScript.gameObject.name, "Cannon Shots/", shootScript.gameObject.GetComponent<AudioSource>().clip.name, shootScript.gameObject.GetComponent<AudioSource>().volume);
						if (options.chromaticAberration) {
							StartCoroutine(shootScript.FadeAberration());
						}
						Fire(shootScripts.IndexOf(shootScript));
						StartCoroutine(shootScript.Cooldown());
					}
				}
			}
		}
	}

	void FixedUpdate () {
		if (!photonView.isMine) return; // exit FixedUpdate function if player isn't ours (multiplayer)

		if (outOfArena) {
			Vector2 direction = (-transform.position).normalized;
			GetComponent<Rigidbody2D>().AddForce(direction * 7.5f); // apply force towards centre of arena
		}

		if (!pauseMenu.activeInHierarchy && !builderMenu.activeInHierarchy) { // if not paused and not in player builder
			if (Input.GetKey("w")) { // if "w" key pressed
				rib.AddRelativeForce(new Vector2(0, totalSpeed)); // move forward at totalSpeed (base speed + any additional speed upgrades)
				movingForwards = true;
			} else {
				movingForwards = false;
			}

			if (Input.GetKey("s")) { // if "s" key pressed
				rib.AddRelativeForce(new Vector2(0, (-totalSpeed / 4))); // move backward at totalSpeed / 4
			}

			if (Input.GetKey("a")) { // if "a" key pressed
				rib.AddTorque(totalTurnSpeed); // turn left at totalTurnSpeed
			}

			if (Input.GetKey("d")) { // if "d" key pressed
				rib.AddTorque(-totalTurnSpeed); // turn right at totalTurnSpeed
			}
		} else {
			movingForwards = false;
		}
	}

	public void AffectHealth(int amount) {
        pixels += amount; // adds amount to pixels, use negative value for damaging player

		if (pixels <= 0) {
			Debug.Log (name + " died!");
			Die();
		}
    }

	void Die () {
		PhotonNetwork.DestroyPlayerObjects(photonView.owner);
		deathCanvas.SetActive(true);
	}

	public void SpawnAttachment (string bAName, float posX, float posY) {
		//Object loadedAttachment = Resources.Load ("Attachments/" + bAName);
		//GameObject attachment = (GameObject)Instantiate (loadedAttachment, transform.position, transform.rotation);
		GameObject attachment = PhotonNetwork.Instantiate(bAName, transform.position, transform.rotation, 0);

		attachmentCount++;
		attachments.Add(attachment);

		photonView.RPC("ConfigureAttachment", PhotonTargets.AllBufferedViaServer, attachment.GetPhotonView().viewID, posX, posY);
	}

	[PunRPC]
	void ConfigureAttachment (int attViewID, float posX, float posY) {
		GameObject attachment = PhotonView.Find(attViewID).gameObject;
		attachment.transform.SetParent(transform);
		attachment.GetComponent<AttachToPlayer>().localPos = new Vector3(posX, posY, 0);
		attachment.transform.localPosition = attachment.GetComponent<AttachToPlayer>().localPos;
		attachment.GetComponent<AttachToPlayer>().target = transform;
		attachment.GetComponent<AttachToPlayer>().original = Resources.Load("Attachments/Resources/" + attachment.name.Replace("(Clone)", "").Trim());
		attachment.tag = "Attachment";
		string tempName = attachment.name.Replace("(Clone)", "").Trim();
		if (attachment.GetComponent<AttachToPlayer>().sameColorAsShip) {
			attachment.GetComponent<SpriteRenderer>().material.color = GetComponent<SpriteRenderer>().material.color;
		}
		attachment.name = name + tempName + attachmentCount;

		Physics2D.IgnoreCollision(attachment.GetComponent<Collider2D>(), GetComponent<Collider2D>());
		foreach (Transform child in transform) {
			if (child.gameObject != attachment && child.GetComponent<Collider2D>()) {
				Physics2D.IgnoreCollision(attachment.GetComponent<Collider2D>(), child.GetComponent<Collider2D>());
			}
		}

		if (attachment.GetComponent<Shoot>()) {
			shootScripts.Add(attachment.GetComponent<Shoot>());
		}
	}

	public void Fire (int shootScriptIndex) {
		if (!photonView.isMine) return; // exit Fire function if player isn't ours (multiplayer)

		Shoot shootScript = shootScripts[shootScriptIndex]; // get Shoot script of cannon that shot

		GameObject shot = PhotonNetwork.Instantiate(shootScript.projectile.name, shootScript.transform.position + (shootScript.transform.up / 3), shootScript.transform.rotation, 0); // create projectile object
		Physics2D.IgnoreCollision(shot.GetComponent<Collider2D>(), GetComponent<Collider2D>()); // ignore collision between projectile and our own ship (avoid shooting ourself)

		shot.transform.rotation = shootScript.transform.rotation; // make projectile start facing correct direction
		shot.transform.rotation = Quaternion.Euler(0, 0, shootScript.transform.rotation.eulerAngles.z + Random.Range(-shootScript.aimCone, shootScript.aimCone));
		shot.GetComponent<Projectile>().speed = ((totalSpeed / rib.drag) - Time.fixedDeltaTime * totalSpeed) / rib.mass * shootScript.projectileSpeedMultiplier; // speed = player speed + projectile speed
		shot.GetComponent<Projectile>().torque = shootScript.projectileTorque; // allow projectile to rotate
		shot.GetComponent<Projectile>().knockback = shootScript.projectileKnockback / 10; // set knockback applied to objects collided with
		shot.GetComponent<Projectile>().damage = shootScript.projectileDamage; // set damage
		shot.GetComponent<Projectile>().travelDistance = shootScript.projectileTravelDistance; // set how far projectile can travel before it is destroyed

		photonView.RPC("SetupShot", PhotonTargets.AllBufferedViaServer, shot.GetPhotonView().viewID, shootScriptIndex); // setup shot for all players

		shot.GetComponent<Projectile>().StartShot(shootScript, rib.velocity, this); // start projectile movement
	}

	[PunRPC]
	void SetupShot (int shotViewID, int shootScriptIndex) {
		GameObject shot = PhotonView.Find(shotViewID).gameObject; // find projectile by its multiplayer object ID
		Shoot shootScript = shootScripts[shootScriptIndex]; // get Shoot script of cannon that shot
		
		//Debug.Log(shootScript.gameObject.name);

		shot.name = name + shootScript.projectile.name; // set projectile name
		shot.tag = "Projectile"; // set projectile tag

		Physics2D.IgnoreCollision(shot.GetComponent<Collider2D>(), GetComponent<Collider2D>()); // ignore collision between projectile and our own ship
		foreach (GameObject attachment in attachments) {
			Physics2D.IgnoreCollision(shot.GetComponent<Collider2D>(), attachment.GetComponent<Collider2D>()); // ignore collision between projectile and our own attachments
		}

		GameObject[] otherProjectiles = GameObject.FindGameObjectsWithTag("Projectile");
		foreach (GameObject go in otherProjectiles) {
			Physics2D.IgnoreCollision(shot.GetComponent<Collider2D>(), go.GetComponent<Collider2D>()); // ignore collision with other projectiles
		}
	}

	[PunRPC]
	public void FadeIn () {
		flame.SetActive(true);
		flameTrail.GetComponent<ParticleSystem> ().Play ();
		flame.GetComponent <SpriteRenderer>().enabled = true;
		flame.transform.Find ("Light").gameObject.GetComponent<Light> ().range = 10f;
		/*currentLerpTime = Time.deltaTime;
		//	print ("lerping up");
		while (currentLerpTime > Time.deltaTime - lerpTime && movingForwards) {
			flame.transform.FindChild("Light").gameObject.GetComponent<Light> ().range = Mathf.Lerp (flame.transform.FindChild("Light").gameObject.GetComponent <Light>().range, 10, (Time.deltaTime - currentLerpTime) / lerpTime);
			yield return new WaitForSeconds (0);
		}*/
	}

	[PunRPC]
	public void FadeOut () {
		flame.transform.Find("Light").gameObject.GetComponent<Light> ().range = 2.3f;
		flameTrail.GetComponent<ParticleSystem> ().Stop ();
		flame.SetActive(false);
	}
}