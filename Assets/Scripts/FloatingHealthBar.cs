using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class FloatingHealthBar : NetworkBehaviour {

	private float health;
	private SpriteRenderer rend;

	// Use this for initialization
	void Start () {
		rend = transform.FindChild ("HealthBarOthers").GetComponent<SpriteRenderer> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (!isServer)
			return;

		RpcUpdateCutoff ();
	}

	void LateUpdate () {
		rend.gameObject.transform.position = transform.position - new Vector3 (1f, -1.75f, 0f);
		rend.gameObject.transform.rotation = Quaternion.Euler (Vector3.zero);
	}

	[ClientRpc]
	public void RpcUpdateCutoff () {
		health = Mathf.Clamp (1f - GetComponent<Player> ().pixels / 200f, 0.5f, 1f);
		if (health != 1f) {
			rend.enabled = true;
			rend.material.SetFloat ("_Cutoff", health);
		} else {
			rend.enabled = false;
		}
	}
}