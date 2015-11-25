using UnityEngine;
using System.Collections;

public class ThrustFlicker : MonoBehaviour {

	public float minIntensity;
	public float maxIntensity;
	public float minGreen;
	public float maxGreen;
	public float maxGreenChange;
	private bool canFlicker;
	
	void OnEnable () {
		canFlicker = true;
	}
	
	void Update () {
		if (canFlicker) {
			canFlicker = false;
			StartCoroutine(Flicker ());
		}
	}
	
	IEnumerator Flicker () {
		GetComponent<Light> ().intensity = Random.Range (minIntensity, maxIntensity);
		GetComponent<Light> ().color = new Color (GetComponent<Light> ().color.r, Mathf.Clamp (GetComponent<Light> ().color.g + Random.Range (-maxGreenChange, maxGreenChange), minGreen, maxGreen), GetComponent<Light> ().color.b);

		
		yield return new WaitForSeconds (0.1f);
		canFlicker = true;
	}
}