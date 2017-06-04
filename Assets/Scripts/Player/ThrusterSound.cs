using UnityEngine;
using System.Collections;

public class ThrusterSound : Photon.MonoBehaviour {

	public AudioClip thrustSoundStart;
	private NetOperations netOps;

    void OnEnable() {
		netOps = GameObject.Find ("NetworkManager").GetComponent<NetOperations> ();
		netOps.gameObject.GetPhotonView ().RPC ("RPCPlayOneShot", PhotonTargets.All, transform.parent.gameObject.GetPhotonView ().viewID, this.name, "", thrustSoundStart.name, 1f);
//		netOps.PlayOneShot(thrustSoundStart, 1);
//		thrustFX.Play();
		netOps.gameObject.GetPhotonView ().RPC ("RPCPlay", PhotonTargets.All, transform.parent.gameObject.GetPhotonView ().viewID, this.name);
	}

	void OnDisable () {
		netOps.gameObject.GetPhotonView ().RPC ("RPCStop", PhotonTargets.All, transform.parent.gameObject.GetPhotonView ().viewID, this.name);
	}
}