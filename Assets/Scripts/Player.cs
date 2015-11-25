using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Player : NetworkBehaviour {

	public float speed;
	public float turnSpeed;
	
	private Rigidbody2D rib;
	private bool movingForwards;
	public GameObject flame;
	private float currentLerpTime;
	public float lerpTime;
	
	private bool canFadeIn;
	private bool canFadeOut;
	
	[SyncVar] public int pixels;
	
	private Shoot shootScript;
	[HideInInspector] public bool canShoot;

	void Start () {
		rib = GetComponent<Rigidbody2D> ();
		shootScript = GetComponentInChildren<Shoot> ();
		canFadeIn = true;
		canShoot = false;
		StartCoroutine (Cooldown ());
	}

	void Update () {
		if (!isLocalPlayer)
			return;
	
		if (Input.GetKey("w")) {
			rib.AddRelativeForce(new Vector2(0, speed));
			movingForwards = true;
		} else {
			movingForwards = false;
		}

		if (Input.GetKey("s")) {
			rib.AddRelativeForce(new Vector2(0, (-speed / 4)));
		}

		if (Input.GetKey("a")) {
			rib.AddTorque(turnSpeed);
		}

		if (Input.GetKey("d")) {
			rib.AddTorque(-turnSpeed);
		}

		if (canFadeIn && movingForwards) {
			canFadeIn = false;
			canFadeOut = true;
			StartCoroutine (FlameFadeIn ());
		}
		if (canFadeOut && !movingForwards) {
			canFadeOut = false;
			canFadeIn = true;
			//  FlameFadeOut ();
			flame.transform.FindChild("Light").gameObject.GetComponent<Light> ().range = 2.3f;
			flame.SetActive(false);
		}

		if (Input.GetMouseButton(0)) {
			if (canShoot) {
				canShoot = false;
				shootScript.sfx.PlayOneShot (shootScript.sfx.clip, shootScript.sfx.volume);
				StartCoroutine (shootScript.FadeAberration());
				CmdFire ();
				StartCoroutine (Cooldown ());
			}
		}
	}

	public void AffectHealth(int amount)
    {
        if (!isServer)
            return;

        pixels += amount;

		if (pixels <= 0) {
			Debug.Log (name + " died!");
		}
    }

	[Command]
	public void CmdFire () {
		GameObject shot = (GameObject) Instantiate (shootScript.projectile, shootScript.gameObject.transform.position + (shootScript.gameObject.transform.up / 3), shootScript.gameObject.transform.rotation);
		shot.name = name + shootScript.projectile.name;
		shot.tag = "Projectile";
		shot.GetComponent<Projectile> ().speed = shootScript.projectileSpeed;
		shot.GetComponent<Projectile> ().knockback = shootScript.projectileKnockback / 10;
		shot.GetComponent<Projectile> ().damage = shootScript.projectileDamage;
		shot.GetComponent<Projectile> ().travelDistance = shootScript.projectileTravelDistance;
		Physics2D.IgnoreCollision(shot.GetComponent<Collider2D> (), GetComponent<Collider2D> ());

		NetworkServer.Spawn (shot);
		RpcIgnoreCollisions (shot);
	}

	[ClientRpc]
	public void RpcIgnoreCollisions (GameObject shot) {
		Physics2D.IgnoreCollision(shot.GetComponent<Collider2D> (), GetComponent<Collider2D> ());
		shot.name = name + shootScript.projectile.name;
	}

	IEnumerator Cooldown () {
		yield return new WaitForSeconds (shootScript.cooldown);
		canShoot = true;
	}

	IEnumerator FlameFadeIn () {
		flame.SetActive(true);
		flame.GetComponent <SpriteRenderer>().enabled = true;
		currentLerpTime = Time.time;
		//	print ("lerping up");
		while (currentLerpTime > Time.time - lerpTime && movingForwards) {
			flame.transform.FindChild("Light").gameObject.GetComponent<Light> ().range = Mathf.Lerp (flame.transform.FindChild("Light").gameObject.GetComponent <Light>().range, 10, (Time.time - currentLerpTime) / lerpTime);
			yield return new WaitForSeconds (0);
		}
	}
}