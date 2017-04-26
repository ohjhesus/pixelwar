using UnityEngine;
using UnityStandardAssets.ImageEffects;
using System.Collections;

public class Shoot : MonoBehaviour {
	
	public float cooldown;
	public GameObject projectile;
	public float projectileSpeedMultiplier;
	private float totalPlayerSpeed;
	public float projectileTorque;
	public float projectileKnockback;
	public int projectileDamage;
	public float projectileTravelDistance;

	public float aberration;
	public float lerpTime;

	public bool canShoot;

	void Start () {
		projectileDamage = -projectileDamage;
		canShoot = false;
		StartCoroutine (Cooldown ());
	}

	public void RemoveFromLists () {
		GetComponent<AttachToPlayer> ().target.GetComponent<Player> ().shootScripts.Remove (this);
	}

	public IEnumerator Cooldown () {
		yield return new WaitForSeconds (cooldown);
		canShoot = true;
	}

	public IEnumerator FadeAberration () {
		Camera.main.GetComponent<VignetteAndChromaticAberration> ().chromaticAberration = aberration;
		float currentLerpTime = 0;
		while (currentLerpTime <= lerpTime) {
			currentLerpTime += Time.deltaTime;
			Camera.main.GetComponent<VignetteAndChromaticAberration> ().chromaticAberration = Mathf.Lerp (Camera.main.GetComponent<VignetteAndChromaticAberration> ().chromaticAberration, 0f, currentLerpTime / lerpTime);
			yield return new WaitForSeconds (0);
		}
	}
}
