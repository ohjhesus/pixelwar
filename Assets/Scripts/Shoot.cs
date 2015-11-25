using UnityEngine;
using UnityStandardAssets.ImageEffects;
using UnityEngine.Networking;
using System.Collections;

public class Shoot : NetworkBehaviour {
	
	public float cooldown;
	public GameObject projectile;
	public float projectileSpeed;
	public float projectileKnockback;
	public int projectileDamage;
	public float projectileTravelDistance;

	public float aberration;
	public float lerpTime;

	[HideInInspector]
	public AudioSource sfx;
	

	void Start () {
		sfx = GetComponent<AudioSource> ();
		GetComponentInParent<FollowMouse> ().cannons.Add (gameObject);
	}
	
	void Update () {

	}

	public IEnumerator FadeAberration () {
		Camera.main.GetComponent<VignetteAndChromaticAberration> ().chromaticAberration = aberration;
		float currentLerpTime = Time.time;
		while (currentLerpTime > Time.time - cooldown) {
			Camera.main.GetComponent<VignetteAndChromaticAberration> ().chromaticAberration = Mathf.Lerp (Camera.main.GetComponent<VignetteAndChromaticAberration> ().chromaticAberration, 0f, (Time.time - currentLerpTime) / cooldown);
			yield return new WaitForSeconds (0);
		}
	}
}