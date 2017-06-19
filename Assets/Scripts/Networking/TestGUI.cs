using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestGUI : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	private void OnGUI() {
		GUI.Label(new Rect (0, 10, 200, 50), "Resent Reliable Commands: " + PhotonNetwork.ResentReliableCommands.ToString());
	}
}
