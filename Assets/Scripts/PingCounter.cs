using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PingCounter : MonoBehaviour {

	// Use this for initialization
	void Start () {
		StartCoroutine (UpdatePing ());
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	IEnumerator UpdatePing () {
		GetComponent<Text> ().text = PhotonNetwork.networkingPeer.RoundTripTime + " ms";
		yield return new WaitForSeconds (1);
		StartCoroutine (UpdatePing ());
	}
}