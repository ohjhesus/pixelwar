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

	private Rigidbody2D rib;

	// Sync SO health
	void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
		if (stream.isWriting) {
			stream.SendNext(health);
		} else {
			health = (int)stream.ReceiveNext();
		}
	}

	// Use this for initialization
	void Start () {
		
	}

	void FinishedSetup () {
		transform.localScale = new Vector3(size, size, 1);
		rib = GetComponent<Rigidbody2D>();
		rib.AddForce(direction.normalized * speed, ForceMode2D.Impulse);
		rib.AddTorque(rotSpeed / 100);
	}

	// Update is called once per frame
	void Update () {
		/*transform.rotation = Quaternion.Euler (0, 0, transform.rotation.eulerAngles.z + rotSpeed);
		transform.position += new Vector3 (direction.x, direction.y, 0) * speed / 100;*/
		if (transform.position.x > 120 || transform.position.x < -120 || transform.position.y > 120 || transform.position.y < -120) {
			//magic number to destroy so's without drops
			AffectHealth (-42069);
		}
	}

	public void SpawnPixels (int amount) {
//		Debug.Log (amount);
		while (amount >= 10) {
			GameObject pt = (GameObject)PhotonNetwork.Instantiate ("10Pixel", new Vector3 (transform.position.x, transform.position.y, -1), transform.rotation, 0);
			pt.GetComponent<Rigidbody2D> ().AddForce (new Vector2 (Random.Range (-1f, 1f) * 100f, Random.Range (-1f, 1f) * 100f));
			amount -= 10;
		}
		while (amount >= 5) {
			GameObject pf = (GameObject)PhotonNetwork.Instantiate ("5Pixel", new Vector3 (transform.position.x, transform.position.y, -1), transform.rotation, 0);
			pf.GetComponent<Rigidbody2D> ().AddForce (new Vector2 (Random.Range (-1f, 1f) * 100f, Random.Range (-1f, 1f) * 100f));
			amount -= 5;
		}
		while (amount >= 1) {
			GameObject po = (GameObject)PhotonNetwork.Instantiate ("1Pixel", new Vector3 (transform.position.x, transform.position.y, -1), transform.rotation, 0);
			po.GetComponent<Rigidbody2D> ().AddForce (new Vector2 (Random.Range (-1f, 1f) * 100f, Random.Range (-1f, 1f) * 100f));
			amount -= 1;
		}
	}

	public void AffectHealth (int amount) {
		health += amount;
//		Debug.Log (name + " health: " + health);

		if (health <= 0) {
			Debug.Log (name + " destroyed");
			GameObject explosion = (GameObject) Instantiate (Resources.Load ("Explosion"), new Vector3 (transform.position.x, transform.position.y, -2), transform.rotation);
			explosion.transform.localScale = new Vector3 (size * 10, size * 10, 1);

			if (amount != -42069) {
				SpawnPixels (Mathf.FloorToInt (size * 10));
			}
			
			if (transform.FindChild("Trail")) {
				photonView.RPC("DestroySOWithTrail", PhotonTargets.AllBufferedViaServer);
			} else {
				if (PhotonNetwork.isMasterClient) {
					PhotonNetwork.Destroy(gameObject);
				}
			}
		}
	}

	[PunRPC]
	void DestroySOWithTrail () {
		Destroy(transform.FindChild("Trail"), transform.FindChild("Trail").GetComponent<ParticleSystem>().main.startLifetime.constantMax);
		transform.FindChild("Trail").SetParent(null);

		if (PhotonNetwork.isMasterClient) {
			PhotonNetwork.Destroy(gameObject);
		}
	}

	void OnCollisionEnter2D (Collision2D coll) {
		if (coll.gameObject.tag == "Projectile") {
			AffectHealth (coll.gameObject.GetComponent<Projectile> ().damage);
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
		Debug.Log("spawning space object - size: " + size + "; health: " + health + "; speed: " + speed + "; torque: " + rotSpeed);

		//Begin movement
		FinishedSetup();
	}

	[PunRPC]
	void CreateExplosion () {

	}
}
