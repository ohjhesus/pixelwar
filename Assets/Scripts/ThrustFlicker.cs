using UnityEngine;
using System.Collections;

public class ThrustFlicker : MonoBehaviour {

	public float minIntensity;
	public float maxIntensity;
	public float minGreen;
	public float maxGreen;
	public float maxGreenChange;
	private bool canFlicker;
	private Light thrusterLight;

	void Start () {
		thrusterLight = GetComponent<Light>();
	}
	
	void OnEnable () {
		canFlicker = true;
	}
	
	void Update () {
		if (canFlicker) {
			canFlicker = false;
			thrusterLight.intensity = Random.Range(minIntensity, maxIntensity);
			thrusterLight.color = new Color(thrusterLight.color.r, Mathf.Clamp(thrusterLight.color.g + Random.Range(-maxGreenChange, maxGreenChange), minGreen, maxGreen), thrusterLight.color.b);
			StartCoroutine(Cooldown ());
		}
	}
	
	IEnumerator Cooldown () {
		yield return new WaitForSeconds (0.1f);
		canFlicker = true;
	}
}