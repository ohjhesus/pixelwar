using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

class NameSync : NetworkBehaviour {

	[SyncVar(hook="OnName")]
	public string myName;
	
	void OnName(string newName)
	{
		name = newName;
		myName = newName;
	}
	
	public override void OnStartClient()
	{
		name = myName;
	}
}