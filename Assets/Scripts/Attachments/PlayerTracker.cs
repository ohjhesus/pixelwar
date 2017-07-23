using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTracker : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (transform.parent.tag == "Player") {
			transform.parent.Find ("ClosestPlayer").GetComponent<FollowPlayer> ().isEnabled = true;
		}
	}
}