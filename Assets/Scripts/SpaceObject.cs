using UnityEngine;
using System.Collections;

public class SpaceObject : Photon.MonoBehaviour {

	[HideInInspector] public Vector2 direction;
	[HideInInspector] public float size;
	[HideInInspector] public int health;
	[HideInInspector] public float speed;
	[HideInInspector] public float rotSpeed;

	public float minSize = 0.5f;
	public float maxSize = 2f;
	public float minSpeed = 10f;
	public float maxSpeed = 50f;

	public Texture2D source;

	private Rigidbody2D rib;

	private bool canDestroy = true;

	public AudioClip[] rockHitClips;

	// Sync SO health
	void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
		if (stream.isWriting) {
			stream.SendNext(health);
		} else {
			health = (int)stream.ReceiveNext();
			AffectHealth(0);
		}
	}

	void FinishedSetup () {
		transform.localScale = new Vector3(size, size, 1);
		rib = GetComponent<Rigidbody2D>();
		rib.AddForce(direction.normalized * speed, ForceMode2D.Impulse);
		rib.AddTorque(rotSpeed / 100);
	}

	// Update is called once per frame
	void Update () {
		if (!PhotonNetwork.isMasterClient) return;

		/*transform.rotation = Quaternion.Euler (0, 0, transform.rotation.eulerAngles.z + rotSpeed);
		transform.position += new Vector3 (direction.x, direction.y, 0) * speed / 100;*/
		if (canDestroy) {
			if (transform.position.x > 120 || transform.position.x < -120 || transform.position.y > 120 || transform.position.y < -120) {
				canDestroy = false;

				//magic number to destroy so's without drops
				AffectHealth(-42069);
			}
		}
	}

	[PunRPC]
	public void SpawnPixels (int amount) {
//		Debug.Log (amount);
		while (amount >= 10) { // while amount of pixels to create is greater than or equal to 10
			GameObject pt = (GameObject)PhotonNetwork.InstantiateSceneObject ("10Pixel", new Vector3 (transform.position.x, transform.position.y, -1), transform.rotation, 0, null);
			pt.GetComponent<Rigidbody2D> ().AddForce (new Vector2 (Random.Range (-1f, 1f) * 100f, Random.Range (-1f, 1f) * 100f));
			amount -= 10; // create pixel object(s) with value of 10, apply random force to simulate explosion, subtract 10 from amount of pixels to create
		}
		while (amount >= 5) { // while remaining amount of pixels to create is greater than or equal to 5
			GameObject pf = (GameObject)PhotonNetwork.InstantiateSceneObject ("5Pixel", new Vector3 (transform.position.x, transform.position.y, -1), transform.rotation, 0, null);
			pf.GetComponent<Rigidbody2D> ().AddForce (new Vector2 (Random.Range (-1f, 1f) * 100f, Random.Range (-1f, 1f) * 100f));
			amount -= 5; // create pixel object(s) with value of 5, apply random force to simulate explosion, subtract 5 from amount of pixels to create
		}
		while (amount >= 1) { // while remaining amount of pixels to create is greater than or equal to 1
			GameObject po = (GameObject)PhotonNetwork.InstantiateSceneObject ("1Pixel", new Vector3 (transform.position.x, transform.position.y, -1), transform.rotation, 0, null);
			po.GetComponent<Rigidbody2D> ().AddForce (new Vector2 (Random.Range (-1f, 1f) * 100f, Random.Range (-1f, 1f) * 100f));
			amount -= 1; // create pixel object(s) with value of 1, apply random force to simulate explosion, subtract 1 from amount of pixels to create
		}
	}

	public void AffectHealth (int amount) {
		if (!PhotonNetwork.isMasterClient) return;

		health += amount;
//		Debug.Log (name + " health: " + health);

		if (health <= 0) {
			Debug.Log (name + " destroyed");
			GameObject explosion = (GameObject) Instantiate (Resources.Load ("Explosion"), new Vector3 (transform.position.x, transform.position.y, -2), transform.rotation);
			explosion.transform.localScale = new Vector3 (size * 10, size * 10, 1);

			if (amount != -42069) {
				//photonView.RPC("SpawnPixels", PhotonTargets.MasterClient, Mathf.FloorToInt (size * 25));
				SpawnPixels(Mathf.FloorToInt(size * 25));
			}
			
			if (transform.Find("Trail")) {
				photonView.RPC("DestroySOWithTrail", PhotonTargets.AllBufferedViaServer);
				//DestroySOWithTrail();
			} else {
				//photonView.RPC("DestroySO", PhotonTargets.MasterClient);
				DestroySO();
			}
		}
	}

	[PunRPC]
	void DestroySO() {
		PhotonNetwork.Destroy(gameObject);
	}

	[PunRPC]
	void DestroySOWithTrail () {
		if (transform.Find("Trail") != null) {
			Destroy(transform.Find("Trail").gameObject, transform.Find("Trail").GetComponent<ParticleSystem>().main.startLifetime.constantMax);
			transform.Find("Trail").SetParent(null);
			SplitSprite();

			if (PhotonNetwork.isMasterClient) {
				DestroySO();
			}
		}
	}

	void OnCollisionEnter2D (Collision2D coll) {
		if (coll.gameObject.tag == "Projectile") {
			AffectHealth (coll.gameObject.GetComponent<Projectile> ().damage);
			if (rockHitClips == null) {
				rockHitClips = (AudioClip[])Resources.LoadAll("Rock hits/");
			}
			AudioClip hitClip = rockHitClips [Random.Range (0, rockHitClips.Length)];
			photonView.RPC ("RPCPlayOneShot", PhotonTargets.All, coll.gameObject.GetPhotonView().viewID, "Explosion", "Rock Hits/", hitClip.name + ".wav", 1f);
			coll.gameObject.GetComponent<Projectile> ().Explode();
		}
	}

	// RPCs

	[PunRPC]
	public void SetupSO (Vector2 tempDirection, float tempSize, int tempHealth, float tempSpeed, float tempRotSpeed) {
		//spaceObject.tag = "SpaceObject";
		direction = tempDirection;
		size = tempSize;
		speed = tempSpeed;
		health = tempHealth;
		rotSpeed = tempRotSpeed;

		//Log spaceObject stats
		//Debug.Log("spawning space object - size: " + size + "; health: " + health + "; speed: " + speed + "; torque: " + rotSpeed);

		//Begin movement
		FinishedSetup();
	}

	void SplitSprite() {
		for (int i = 0; i < 4; i++) {
			for (int j = 0; j < 4; j++) {
				Sprite newSprite = Sprite.Create(source, new Rect(i * 128, j * 128, 128, 128), new Vector2(0.5f, 0.5f));
				GameObject n = new GameObject();
				SpriteRenderer sr = n.AddComponent<SpriteRenderer>();
				sr.sortingLayerName = "Player";
				sr.sortingOrder = 1;
				sr.sprite = newSprite;
				n.transform.localPosition = new Vector3(i * 2, j * 2, 0);
				n.transform.parent = transform;
			}
		}
	}
}
