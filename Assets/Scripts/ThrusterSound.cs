using UnityEngine;
using System.Collections;

public class ThrusterSound : MonoBehaviour {

	private AudioSource thrustFX;
	public AudioClip thrustSoundStart;

    void OnEnable() {
		thrustFX = GetComponent<AudioSource>();
		thrustFX.PlayOneShot(thrustSoundStart, 1);
		thrustFX.Play();
	}

	void OnDisable () {
		thrustFX.Stop();
	}
}