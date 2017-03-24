using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PixelsRemaining : MonoBehaviour {

	public int pixelsRemaining;

	// Use this for initialization
	void Start () {
	
	}

	public void UpdateCounter () {
		GetComponent<Text> ().text = "PIXELS REMAINING: " + pixelsRemaining;
	}

	// Update is called once per frame
	void Update () {
		
	}
}