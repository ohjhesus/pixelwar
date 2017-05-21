using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaveGameButton : MonoBehaviour {

	private NetMgr netMgr;

	// Use this for initialization
	void OnEnable () {
		netMgr = GameObject.Find ("NetworkManager").GetComponent<NetMgr> ();
	}

	public void LeaveGame () {
		// are you sure? implement

		netMgr.Disconnect ();
	}

	// Update is called once per frame
	void Update () {
		
	}
}
