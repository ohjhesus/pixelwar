using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GetResolutions : MonoBehaviour {

	private Dropdown dd;
	public List<string> resolutions;

	// Use this for initialization
	void Start () {
		dd = GetComponent<Dropdown> ();
		resolutions = new List<string> ();
		dd.ClearOptions ();
		foreach (Resolution res in Screen.resolutions) {
			resolutions.Add (res.width.ToString () + "x" + res.height.ToString ());
		}

		dd.AddOptions (resolutions);
		GetComponent <UIInteractions> ().DefaultRes ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}