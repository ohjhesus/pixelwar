using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class PlayerDisplay : MonoBehaviour {

	public Object modelPrefab;
	[HideInInspector] public GameObject playerModel;

	// Use this for initialization
	void Start () {
		
	}

	void OnEnable () {
		if (playerModel == null) {
			playerModel = (GameObject)Instantiate (modelPrefab, new Vector3 (0, 0, 1f), transform.rotation);
			playerModel.transform.SetParent (transform);
			playerModel.name = "PlayerModel";
		}
		playerModel.SetActive (true);
		playerModel.transform.localScale = Vector3.one;
	}

	void OnDisable () {
		foreach (Transform child in transform) {
//			Destroy (child.gameObject);
			child.gameObject.SetActive(false);
			foreach (Transform prevAtt in child) {
				if (prevAtt.tag == "Attachment") {
					prevAtt.tag = "PreviousAttachment";
				}
			}
		}
	}

	// Update is called once per frame
	void Update () {
		if (playerModel != null) {
			playerModel.transform.localPosition = new Vector3(0, 0, -0.1f);
		}
	}
}