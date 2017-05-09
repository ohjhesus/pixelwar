using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LANPanel : MonoBehaviour {

	public GameObject lanPanel;
	public NetMgr netMgr;

	public string address;
	public string port;

	public Text addressText;
	public Text portText;

	// Use this for initialization
	void Start () {
		lanPanel.SetActive (false);
	}

	public void TogglePanel () {
		lanPanel.SetActive (!lanPanel.activeInHierarchy);
	}

	public void SetAddress () {
		address = addressText.text;
	}

	public void SetPort () {
		port = portText.text;
	}

	public void FindLAN () {
		int tempPort = 5055;
		try {
			Debug.Log("parsing port");
			tempPort = int.Parse(port);
		} catch (System.Exception e) {
			Debug.LogWarning ("Port text error: " + e.Message + " - " + port);
		}
		netMgr.StartLANMultiplayer (address, tempPort);
	}

	// Update is called once per frame
	void Update () {
		
	}
}
