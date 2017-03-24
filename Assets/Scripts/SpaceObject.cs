using UnityEngine;
using System.Collections;

public class SpaceObject : MonoBehaviour {

	[HideInInspector] public float size;
	[HideInInspector] public int health;
	[HideInInspector] public Vector2 direction;
	[HideInInspector] public float speed;
	[HideInInspector] public float rotSpeed;

	private Rigidbody2D rib;

	private Object pixelOne;
	private Object pixelFive;
	private Object pixelTen;

	// Use this for initialization
	void Start () {
		LoadPixels ();
		transform.localScale = new Vector3 (size, size, 1);
		rib = GetComponent<Rigidbody2D> ();
		rib.AddForce (direction.normalized * speed, ForceMode2D.Impulse);
		rib.AddTorque (rotSpeed / 100);
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

	void LoadPixels () {
		pixelOne = Resources.Load ("Pixels/1");
		pixelFive = Resources.Load ("Pixels/5");
		pixelTen = Resources.Load ("Pixels/10");
	}

	public void SpawnPixels (int amount) {
//		Debug.Log (amount);
		while (amount >= 10) {
			GameObject pt = (GameObject)Instantiate (pixelTen, new Vector3 (transform.position.x, transform.position.y, -1), transform.rotation);
			pt.GetComponent<Rigidbody2D> ().AddForce (new Vector2 (Random.Range (-1f, 1f) * 100f, Random.Range (-1f, 1f) * 100f));
			amount -= 10;
		}
		while (amount >= 5) {
			GameObject pf = (GameObject)Instantiate (pixelFive, new Vector3 (transform.position.x, transform.position.y, -1), transform.rotation);
			pf.GetComponent<Rigidbody2D> ().AddForce (new Vector2 (Random.Range (-1f, 1f) * 100f, Random.Range (-1f, 1f) * 100f));
			amount -= 5;
		}
		while (amount >= 1) {
			GameObject po = (GameObject)Instantiate (pixelOne, new Vector3 (transform.position.x, transform.position.y, -1), transform.rotation);
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
			Destroy(gameObject);
		}
	}

	void OnCollisionEnter2D (Collision2D coll) {
		if (coll.gameObject.tag == "Projectile") {
			AffectHealth (coll.gameObject.GetComponent<Projectile> ().damage);
			coll.gameObject.GetComponent<Projectile> ().Explode();
		}
	}
}
