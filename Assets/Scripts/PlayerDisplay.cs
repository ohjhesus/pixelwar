using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class PlayerDisplay : MonoBehaviour {

	public Object modelPrefab;
	private GameObject playerModel;

	// Use this for initialization
	void Start () {
		
	}

	void OnEnable () {
		playerModel = (GameObject)Instantiate (modelPrefab, new Vector3 (0, 0, -0.1f), transform.rotation);
		playerModel.transform.SetParent (transform);
		playerModel.name = "PlayerModel";
		playerModel.transform.localScale = Vector3.one;
	}

	void OnDisable () {
		foreach (Transform child in transform) {
			Destroy (child.gameObject);
		}
	}

	// Update is called once per frame
	void Update () {
		if (playerModel != null) {
			playerModel.transform.localPosition = new Vector3(0, 0, -0.1f);
		}
	}
}