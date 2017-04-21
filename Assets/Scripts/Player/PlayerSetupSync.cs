using UnityEngine;
using System.Collections;

class PlayerSetupSync : Photon.MonoBehaviour {
	public Color myColor;

	[PunRPC]
	public void SetupPlayer (string playerName, Color playerColor, int startingPixels) {
		Debug.Log("setup");
	}
}