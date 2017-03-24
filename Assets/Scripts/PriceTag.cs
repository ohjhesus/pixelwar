using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PriceTag : MonoBehaviour {

	// Use this for initialization
	void Start () {
		GameObject tempPart;
		tempPart = (GameObject)transform.parent.FindChild ("Image").GetComponent<BuilderTile> ().attachment;
		GetComponent<Text> ().text = tempPart.GetComponent<AttachToPlayer> ().price + "px";
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
