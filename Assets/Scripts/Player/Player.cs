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

	public bool outOfArena;

	public List<GameObject> attachments;

	void Start () {
		if (photonView.isMine) {
			transform.FindChild("HealthBar").gameObject.SetActive(false);
			transform.FindChild("PlayerName").gameObject.SetActive(false);

			pauseMenu = GameObject.Find("GameManager").GetComponent<Pause>().pausePanel;
			builderMenu = GameObject.Find("GameManager").GetComponent<Builder>().builderPanel;
			options = GameObject.Find("GameManager").GetComponent<Options>();
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
			FadeIn ();
			canFadeIn = false;
			canFadeOut = true;
		}
		if (canFadeOut && !movingForwards) {
			FadeOut ();
			canFadeOut = false;
			canFadeIn = true;
		}

		if (!photonView.isMine) return;

		if (Input.GetMouseButton(0)) {
			if (!pauseMenu.activeInHierarchy && !builderMenu.activeInHierarchy) {

				foreach (Shoot shootScript in shootScripts) {
					if (shootScript.canShoot) {
						shootScript.canShoot = false;
						shootScript.gameObject.GetComponent<AudioSource>().PlayOneShot(shootScript.gameObject.GetComponent<AudioSource>().clip, shootScript.gameObject.GetComponent<AudioSource>().volume);
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
		if (!photonView.isMine) return;

		if (outOfArena) {
			Vector2 direction = (-transform.position).normalized;
			GetComponent<Rigidbody2D>().AddForce(direction * 7.5f);
		}

		if (!pauseMenu.activeInHierarchy && !builderMenu.activeInHierarchy) {
			if (Input.GetKey("w")) {
				rib.AddRelativeForce(new Vector2(0, totalSpeed));
				movingForwards = true;
			} else {
				movingForwards = false;
			}

			if (Input.GetKey("s")) {
				rib.AddRelativeForce(new Vector2(0, (-totalSpeed / 4)));
			}

			if (Input.GetKey("a")) {
				rib.AddTorque(totalTurnSpeed);
			}

			if (Input.GetKey("d")) {
				rib.AddTorque(-totalTurnSpeed);
			}
		} else {
			movingForwards = false;
		}
	}

	public void AffectHealth(int amount)
    {
        pixels += amount;

		if (pixels <= 0) {
			Debug.Log (name + " died!");
		}
    }

	public void SpawnAttachment (string bAName, float posX, float posY) {
		Object loadedAttachment = Resources.Load ("Attachments/" + bAName);
		GameObject attachment = (GameObject)Instantiate (loadedAttachment, transform.position, transform.rotation);
//		Vector3 oldScale = attachment.transform.localScale;
//		attachment.transform.parent = transform;
//		attachment.transform.lossyScale = oldScale;
		attachment.transform.SetParent(transform);
		attachment.GetComponent<AttachToPlayer> ().localPos = new Vector3 (posX, posY, 0);
		attachment.transform.localPosition = attachment.GetComponent<AttachToPlayer> ().localPos;
		attachment.GetComponent<AttachToPlayer> ().target = transform;
		attachment.GetComponent<AttachToPlayer> ().original = Resources.Load ("Attachments/" + attachment.name.Replace ("(Clone)", "").Trim ());
		attachment.tag = "Attachment";
		string tempName = attachment.name.Replace ("(Clone)", "").Trim ();
		if (attachment.GetComponent<AttachToPlayer> ().sameColorAsShip) {
			attachment.GetComponent<SpriteRenderer> ().material.color = GetComponent<SpriteRenderer> ().material.color;
		}
		attachment.name = name + tempName + attachmentCount;
		attachmentCount++;

		Physics2D.IgnoreCollision (attachment.GetComponent<Collider2D> (), GetComponent<Collider2D> ());
		foreach (Transform child in transform) {
			if (child.gameObject != attachment && child.GetComponent<Collider2D> ()) {
				Physics2D.IgnoreCollision (attachment.GetComponent<Collider2D> (), child.GetComponent<Collider2D> ());
			}
		}

		if (attachment.GetComponent<Shoot> ()) {
			shootScripts.Add (attachment.GetComponent<Shoot> ());
		}
	}

	public void Fire (int shootScriptIndex) {
		if (!photonView.isMine) return;

		Shoot shootScript = shootScripts[shootScriptIndex];

		GameObject shot = (GameObject)Instantiate(shootScript.projectile, shootScript.gameObject.transform.position + (shootScript.gameObject.transform.up / 3), shootScript.gameObject.transform.rotation);
		shot.name = name + shootScript.projectile.name;
		shot.tag = "Projectile";
		shot.GetComponent<Projectile>().speed = ((totalSpeed / rib.drag) - Time.fixedDeltaTime * totalSpeed) / rib.mass * shootScript.projectileSpeedMultiplier;
		shot.GetComponent<Projectile>().torque = shootScript.projectileTorque;
		shot.GetComponent<Projectile>().knockback = shootScript.projectileKnockback / 10;
		shot.GetComponent<Projectile>().damage = shootScript.projectileDamage;
		shot.GetComponent<Projectile>().travelDistance = shootScript.projectileTravelDistance;

		Physics2D.IgnoreCollision(shot.GetComponent<Collider2D>(), GetComponent<Collider2D>());
		foreach (GameObject attachment in attachments) {
			Physics2D.IgnoreCollision(shot.GetComponent<Collider2D>(), attachment.GetComponent<Collider2D>());
		}

		GameObject[] otherProjectiles = GameObject.FindGameObjectsWithTag("Projectile");
		foreach (GameObject go in otherProjectiles) {
			Physics2D.IgnoreCollision(shot.GetComponent<Collider2D>(), go.GetComponent<Collider2D>());
		}
	}

	public void FadeIn () {
		flame.SetActive(true);
		flame.GetComponent <SpriteRenderer>().enabled = true;
		flame.transform.FindChild ("Light").gameObject.GetComponent<Light> ().range = 10f;
		/*currentLerpTime = Time.deltaTime;
		//	print ("lerping up");
		while (currentLerpTime > Time.deltaTime - lerpTime && movingForwards) {
			flame.transform.FindChild("Light").gameObject.GetComponent<Light> ().range = Mathf.Lerp (flame.transform.FindChild("Light").gameObject.GetComponent <Light>().range, 10, (Time.deltaTime - currentLerpTime) / lerpTime);
			yield return new WaitForSeconds (0);
		}*/
	}

	public void FadeOut () {
		flame.transform.FindChild("Light").gameObject.GetComponent<Light> ().range = 2.3f;
		flame.SetActive(false);
	}
}