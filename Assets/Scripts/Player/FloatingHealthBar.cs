using UnityEngine;
using System.Collections;

public class FloatingHealthBar : MonoBehaviour {

	private float health;
	private SpriteRenderer rend;
	private bool canUpdate;

	// Use this for initialization
	void Start () {
		rend = transform.FindChild ("HealthBar").GetComponent<SpriteRenderer> ();
		canUpdate = true;
	}
	
	// Update is called once per frame
	void Update () {
		if (canUpdate)
			UpdateCutoff ();
	}

	void LateUpdate () {
		rend.gameObject.transform.position = transform.position - new Vector3 (1f, -1.75f, 0f);
		rend.gameObject.transform.rotation = Quaternion.Euler (Vector3.zero);
	}

	public void UpdateCutoff () {
		health = Mathf.Clamp (1f - GetComponent<Player> ().pixels / 200f, 0.5f, 1f);
		if (health != 1f) {
			rend.enabled = true;
			rend.material.SetFloat ("_Cutoff", health);
		} else {
			rend.enabled = false;
		}
	}
}