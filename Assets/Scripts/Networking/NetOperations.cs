using UnityEngine;
using System.Collections;

public class NetOperations : MonoBehaviour {

	// SOUND

	[PunRPC]
	public void RPCPlayOneShot (int targetAudioSourceID, string childName, string path, string clipName, float volume) {
		if (childName == "") {
			PhotonView.Find (targetAudioSourceID).GetComponent<AudioSource> ().PlayOneShot ((AudioClip)Resources.Load (path + clipName), volume);
		} else {
			PhotonView.Find (targetAudioSourceID).transform.Find(childName).GetComponent<AudioSource> ().PlayOneShot ((AudioClip)Resources.Load (path + clipName), volume);
		}
	}

	[PunRPC]
	public void RPCPlay (int targetAudioSourceID, string childName) {
		if (childName == "") {
			PhotonView.Find (targetAudioSourceID).GetComponent<AudioSource> ().Play ();
		} else {
			PhotonView.Find (targetAudioSourceID).transform.Find(childName).GetComponent<AudioSource> ().Play ();
		}
	}

	[PunRPC]
	public void RPCStop (int targetAudioSourceID, string childName) {
		if (childName == "") {
			PhotonView.Find (targetAudioSourceID).GetComponent<AudioSource> ().Stop ();
		} else {
			PhotonView.Find (targetAudioSourceID).transform.Find(childName).GetComponent<AudioSource> ().Stop ();
		}
	}

	// LIGHT

	[PunRPC]
	public void RPCSetLightIntensity (int targetAudioSourceID, string childName, float intensity) {
		if (childName == "") {
			PhotonView.Find (targetAudioSourceID).GetComponent<Light> ().intensity = intensity;
		} else {
			PhotonView.Find (targetAudioSourceID).transform.Find(childName).GetComponent<Light> ().intensity = intensity;
		}
	}
}