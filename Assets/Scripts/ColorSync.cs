using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

class ColorSync : NetworkBehaviour {

	[SyncVar(hook="OnColor")]
	public Color myColor;
	
	void OnColor(Color newColor)
	{
		GetComponent<Renderer>().material.color = newColor;
		myColor = newColor;
	}
	
	public override void OnStartClient()
	{
		GetComponent<Renderer>().material.color = myColor;
	}
}